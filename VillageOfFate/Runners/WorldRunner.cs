using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAi;
using OpenAi.Gpt;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Runners;

public class WorldRunner(
	TimeService time,
	VillagerService villagers,
	VillagerActivityService villagerActivities,
	VillagerActionErrorService villagerActionErrors,
	ActionFactory actionFactory,
	GptUsageService gptUsage,
	OpenApi openApi,
	RandomProvider random,
	StatusBuilder statusBuilder
) : IRunner {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task RunAsync(CancellationToken cancellationToken) {
		try {
			while (!cancellationToken.IsCancellationRequested) {
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
				Content = await statusBuilder.BuildVillagerStatusAsync(villager)
			}
		};
		messages.AddRange(villager.Memories.Select(m => new Message {
			Role = Role.User,
			Content = m.Memory
		}));
		messages.Add(new Message {
			Role = Role.User,
			Content = "Please choose an action befitting your character."
					  + "You can choose to interact with the other villagers, do nothing and observe, or speak to the group (please do so in-character, and use natural language)."
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
		var details = new List<ActivityDto> {
			new IdleActivityDto {
				Duration = random.NextTimeSpan(TimeSpan.FromMinutes(2)),
				Priority = 0,
				StartTime = worldNow
			}
		};
		for (var index = 0; index < calls.Length; index++) {
			var call = calls[index];
			var action = actionFactory.Get(call.Function.Name);
			if (action == null) {
				await villagerActionErrors.LogInvalidAction(villager, call.Function.Name, call.Function.Arguments);
				continue;
			}

			var activity = action.ParseArguments(call.Function.Arguments);
			activity.Villager = villager;
			activity.Priority = index + 1;
			activity.StartTime = worldNow;
			details.Add(activity);
		}

		foreach (var activityDetail in details) {
			await villagerActivities.AddAsync(villager, activityDetail);
		}
	}
}