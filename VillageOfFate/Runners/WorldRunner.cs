using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;
using VillageOfFate.WorldServices;

namespace VillageOfFate.Runners;

public class WorldRunner(
	TimeService time,
	VillagerService villagers,
	VillagerActivityService activities,
	VillagerActionErrorService villagerActionErrors,
	ActionFactory actionFactory,
	RandomProvider random,
	DataContext context,
	VillagerActionService actionService
) : IRunner {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task RunAsync(CancellationToken cancellationToken) {
		try {
			while (!cancellationToken.IsCancellationRequested) {
				context.ChangeTracker.Clear();

				var worldTime = await GetWorldTimeAsync();
				if (worldTime > await GetEndTimeAsync()) {
					await EnsureVillagersHaveActivities();
					await Task.Delay(Interval, cancellationToken);
					continue;
				}

				var now = DateTime.UtcNow;
				var timeSinceLastUpdate = now - worldTime;

				if (timeSinceLastUpdate >= Interval) {
					await EnsureVillagersHaveActivities();
					await SimulateWorld();
					await SetWorldTimeAsync(worldTime + Interval);
				} else {
					// Sleep until it's time for the next update
					var sleepDuration = Interval - timeSinceLastUpdate;
					await Task.Delay(sleepDuration, cancellationToken);
				}
			}
		}
		catch (OperationCanceledException) {
			Console.WriteLine("WorldRunner was cancelled");
		}
		catch (Exception e) {
			await Console.Error.WriteLineAsync($"WorldRunner threw an exception: {e}");
		}
		finally {
			Console.WriteLine("Exiting WorldRunner");
		}
	}

	public async Task<DateTime> GetWorldTimeAsync() => await time.GetAsync(TimeLabel.World);
	public async Task SetWorldTimeAsync(DateTime value) => await time.SetAsync(TimeLabel.World, value);

	public async Task<DateTime> GetEndTimeAsync() =>
		await time.GetAsync(TimeLabel.End, await GetWorldTimeAsync() + TimeSpan.FromMinutes(30));

	private async Task EnsureVillagersHaveActivities() {
		var idleVillagers = await villagers.GetVillagersWithoutActivities();
		foreach (var idleVillager in idleVillagers) {
			await actionService.QueueActionsForVillager(idleVillager);
		}
	}

	private async Task SimulateWorld() {
		foreach (var villager in await villagers.GetAllAsync()) {
			await SimulateVillager(villager);
		}
	}

	private async Task SimulateVillager(VillagerDto villager) {
		var currentTime = await GetWorldTimeAsync();
		var currentActivity = villager.CurrentActivity;
		if (currentActivity == null) {
			if (!villager.ActivityQueue.Any()) {
				await actionService.QueueActionsForVillager(villager);
			}

			var nextActivity = villager.ActivityQueue.FirstOrDefault();
			if (nextActivity == null) return;

			var action = actionFactory.Get(nextActivity.Name);
			nextActivity.StartTime = currentTime;
			nextActivity.Status = ActivityStatus.InProgress;
			await activities.SaveAsync(nextActivity);
			IActionResults beginResult;
			try {
				beginResult = await action.Begin(nextActivity);
			}
			catch (Exception ex) {
				await villagerActionErrors.LogActionBeginError(villager, nextActivity, ex);
				nextActivity.Status = ActivityStatus.Error;
				await activities.SaveAsync(nextActivity);
				return;
			}

			await HandleResult(beginResult, new ReactionData {
				Actor = villager,
				Item = null,
				ActiveActionName = nextActivity.Name.ToActiveString()
			});
			return;
		}

		if (currentActivity.EndTime > currentTime) return;

		var currentAction = actionFactory.Get(currentActivity.Name);
		var endResult = await currentAction.End(currentActivity);
		await activities.CompleteAsync(currentActivity);
		await HandleResult(endResult, new ReactionData {
			Actor = villager,
			Item = null,
			ActiveActionName = currentActivity.Name.ToActiveString()
		});
	}

	private async Task HandleResult(IActionResults result, ReactionData reaction) {
		if (!result.TriggerReactions.Any()) {
			return;
		}

		var currentTime = await GetWorldTimeAsync();
		var selected = random.SelectOne(result.TriggerReactions);
		var currentActivity = selected.CurrentActivity;
		if (currentActivity != null) {
			if (!currentActivity.EndTime.HasValue) throw new NullReferenceException();
			currentActivity.DurationRemaining = currentActivity.EndTime.Value - currentTime;
			currentActivity.Status = ActivityStatus.OnHold;
			await activities.SaveAsync(currentActivity);
		}

		await actionService.QueueActionsForVillager(selected, reaction);
	}
}