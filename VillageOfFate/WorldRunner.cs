using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAi;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate;

public class WorldRunner(
	TimeService time,
	VillagerService villagers,
	VillagerActivityService villagerActivities,
	VillagerActionErrorService villagerActionErrors,
	ActionFactory actionFactory,
	GptUsageService gptUsage,
	ChatGptApi chatGptApi,
	RandomProvider random,
	StatusBuilder statusBuilder
) {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task<DateTime> GetWorldTimeAsync() => await time.GetAsync(TimeLabel.World);
	public async Task SetWorldTimeAsync(DateTime value) => await time.SetAsync(TimeLabel.World, value);

	public async Task<DateTime> GetEndTimeAsync() =>
		await time.GetAsync(TimeLabel.End, await GetWorldTimeAsync() + TimeSpan.FromMinutes(2));

	public async Task RunAsync(CancellationToken cancellationToken) {
		try {
			while (!cancellationToken.IsCancellationRequested) {
				var worldTime = await GetWorldTimeAsync();
				if (worldTime > await GetEndTimeAsync()) {
					await Task.Delay(Interval, cancellationToken);
					continue;
				}

				var now = DateTime.UtcNow;
				var timeSinceLastUpdate = now - worldTime;

				if (timeSinceLastUpdate >= Interval) {
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

	private async Task SimulateWorld() {
		var currentTime = await time.GetAsync(TimeLabel.World);
		var villager = villagers.GetVillagerWithTheShortestCompleteTime();
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
			await villagerActivities.PushCurrentActivityIntoQueue(selected);
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
				Content = statusBuilder.BuildStatusFor(villager)
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

		var response = await chatGptApi.GetChatGptResponseAsync(messages.ToArray(),
						   actionFactory.Actions.Select(a => new GptFunction {
							   Name = a.Name,
							   Description = a.Description,
							   Parameters = a.Parameters
						   }), ToolChoice.Required);
		await gptUsage.AddUsageAsync(response);

		var calls = response.Choices.First().Message.ToolCalls ?? [];
		var details = new List<ActivityDto> {
			new IdleActivityDto {
				Duration = random.NextTimeSpan(TimeSpan.FromMinutes(2))
			}
		};
		var now = await time.GetAsync(TimeLabel.World);
		for (var index = 0; index < calls.Length; index++) {
			var call = calls[index];
			var action = actionFactory.Get(call.Function.Name);
			if (action == null) {
				await villagerActionErrors.LogInvalidAction(villager, call.Function.Name, call.Function.Arguments);
				continue;
			}

			var activity = action.ParseArguments(call.Function.Arguments);
			activity.Villager = villager;
			activity.StartTime = now + index * TimeSpan.FromDays(1);
			details.Add(activity);
		}

		foreach (var activityDetail in details) {
			await villagerActivities.AddAsync(villager, activityDetail);
		}
	}
}