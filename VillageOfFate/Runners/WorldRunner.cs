using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAi;
using OpenAi.Gpt;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Localization;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Runners;

public class WorldRunner(
	TimeService time,
	VillagerService villagers,
	VillagerActivityService villagerActivities,
	VillagerActionErrorService villagerActionErrors,
	EventsService events,
	ActionFactory actionFactory,
	GptUsageService gptUsage,
	OpenApi openApi,
	RandomProvider random,
	StatusBuilder statusBuilder,
	DataContext context,
	Plurality plurality
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
		await time.GetAsync(TimeLabel.End, await GetWorldTimeAsync() + TimeSpan.FromMinutes(2));

	private async Task EnsureVillagersHaveActivities() {
		var idleVillagers = await villagers.GetVillagersWithoutActivities();
		foreach (var idleVillager in idleVillagers) {
			await QueueActionsForVillager(idleVillager);
		}
	}

	private async Task SimulateWorld() {
		var currentTime = await time.GetAsync(TimeLabel.World);
		var villager = villagers.GetVillagerWithTheEarliestCompleteTime();
		if (villager.CurrentActivity == null) {
			await QueueActionsForVillager(villager);
			if (villager.CurrentActivity == null) return;
			var action = actionFactory.Get(villager.CurrentActivity.Name);
			await action.Begin(villager.CurrentActivity);
			return;
		}

		if (villager.CurrentActivity.EndTime > currentTime) return;

		var currentActivity = actionFactory.Get(villager.CurrentActivity.Name);
		var activityResult = await currentActivity.End(villager.CurrentActivity);
		await villagerActivities.PopAsync(villager);

		if (activityResult.TriggerReactions.Any()) {
			var selected = random.SelectOne(activityResult.TriggerReactions);
			await QueueActionsForVillager(selected);

			if (selected.CurrentActivity != null) {
				var action = actionFactory.Get(selected.CurrentActivity.Name);
				await action.Begin(selected.CurrentActivity);
			}
		}
	}

	private async Task QueueActionsForVillager(VillagerDto villager) {
		var messages = new List<Message> {
			new() {
				Role = Role.System,
				Content = "You are an NPC in a village. You have a set of actions you can perform. " +
						  "You can choose to perform any number of these actions sequentially (planning ahead), or do nothing (deferring decisions for later). " +
						  "Please always stay in character, and choose actions that make sense for your character, their current mood, situation, recent events they witnessed, and their memories."
			},
			new() {
				Role = Role.User,
				Content = await statusBuilder.BuildVillagerStatusAsync(villager)
			}
		};
		messages.AddRange(villager.WitnessedEvents.Select(e => new Message {
			Role = Role.User,
			Content = $"[{e.Time}]@{e.Sector.Position} {e.Description}"
		}));
		messages.Add(new Message {
			Role = Role.User,
			Content = "Please choose an action befitting your character."
		});

		var response = await openApi.GetChatGptResponseAsync(messages.ToArray(),
						   actionFactory.Actions.Select(a => new GptFunction {
							   Name = a.Name,
							   Description = a.Description,
							   Parameters = a.Parameters
						   }), ToolChoice.Required);
		await gptUsage.AddUsageAsync(response);

		var calls = response.Choices.First().Message.ToolCalls ?? [];
		var worldNow = await time.GetAsync(TimeLabel.World);
		var details = new List<ActivityDto>();
		for (var index = 0; index < calls.Length; index++) {
			var call = calls[index];
			var action = actionFactory.Get(call.Function.Name);
			if (action == null) {
				await villagerActionErrors.LogInvalidAction(villager, call.Function.Name, call.Function.Arguments);
				continue;
			}

			var activity = await action.ParseArguments(call.Function.Arguments);
			activity.Villager = villager;
			activity.Priority = index + 1;
			activity.StartTime = worldNow + random.NextTimeSpan(TimeSpan.FromMinutes(2));
			details.Add(activity);
		}

		await events.AddAsync(villager,
			$"Decides to perform the following {plurality.Pick(details, "action", "actions")}: {string.Join(", ", details.Select(d => d.Description))}");
		foreach (var activityDetail in details) {
			await villagerActivities.AddAsync(villager, activityDetail);
		}
	}
}