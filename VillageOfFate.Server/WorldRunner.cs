using System;
using System.Threading;
using System.Threading.Tasks;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices;

namespace VillageOfFate.Server;

public class WorldRunner(TimeService time, VillagerService villagers) {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task<DateTime> GetWorldTimeAsync() => await time.GetAsync(TimeLabel.World);
	public async Task SetWorldTimeAsync(DateTime value) => await time.SetAsync(TimeLabel.World, value);

	public async Task<DateTime> GetEndTimeAsync() =>
		await time.GetAsync(TimeLabel.End, await GetWorldTimeAsync() + TimeSpan.FromMinutes(2));

	public async Task RunAsync(CancellationToken cancellationToken) {
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

	private async Task SimulateWorld() {
		var currentTime = await time.GetAsync(TimeLabel.World);
		var villager = await villagers.GetVillagerWithTheShortestCompleteTime();
		if (villager.Activity.EndTime > currentTime) {
			return;
		}

		var activityResult = villager.Activity.OnCompletion();
		if (villager.ActivityQueue.TryPop(out var activity)) {
			activity.StartTime = world.CurrenTime;
			villager.CurrentActivity = activity;
		} else {
			await QueueActionsForVillager(villager, world, chatGptApi, actions, logger, villagers);
			PushCurrentActivityIntoQueue(villager, world);
			villager.CurrentActivity = new IdleActivity(random.NextTimeSpan(TimeSpan.FromMinutes(2)), world);
		}

		if (activityResult.TriggerReactions.Any()) {
			var selected = random.SelectOne(activityResult.TriggerReactions);
			PushCurrentActivityIntoQueue(selected, world);
			await QueueActionsForVillager(selected, world, chatGptApi, actions, logger, villagers);
		}
	}

	private static void PushCurrentActivityIntoQueue(Villager villager, World world) {
		var current = villager.CurrentActivity;
		var remainingTime = current.EndTime - world.CurrenTime;
		current.Duration = remainingTime < TimeSpan.Zero ? TimeSpan.Zero : remainingTime;
		villager.ActivityQueue.Push(current);
	}

	private static async Task QueueActionsForVillager(Villager villager, World world, ChatGptApi chatGptApi,
													  List<IVillagerAction> actions,
													  VillageLogger logger, Villager[] villagers) {
		var messages = new List<Message> {
			new() {
				Role = Role.System,
				Content = string.Join("\n", [
					$"Respond as {villager.Name} would. {villager.GetDescription()}",
					"Keep your gender, age, role, history, and personality in mind.",
					"Act like a real person in a fantasy world. Don't declare your actions, just do them.",
					"# Relationships",
					string.Join("\n",
						villager.GetRelationships().Select(r =>
							$"- {r.Villager.Name}: {r.Villager.GetDescription()} Relation: {r.Relation}")),
					"# Emotions (0% = neutral, 100% = maximum intensity)",
					string.Join("\n", villager.GetEmotions().Select(e => $"- {e.Emotion}: {e.Intensity}%")),
					"# Location",
					$"You are located at Sector Coordinate {villager.SectorLocation}.",
					$"Description: {world.GetSector(villager.SectorLocation).Description}",
					"Items:",
					string.Join("\n",
						world.GetSector(villager.SectorLocation).Items.Select(i => $"- {i.GetSummary()}")),
					"# Status",
					$"- Hunger: {villager.Hunger} (+1 per hour)",
					"# Inventory",
					string.Join("\n", villager.Inventory.Select(i => $"- {i.GetSummary()}"))
				])
			}
		};
		messages.AddRange(villager.GetMemory().Select(h => new Message {
			Role = Role.User,
			Content = h
		}));
		messages.Add(new Message {
			Role = Role.User,
			Content = "Please choose an action befitting your character."
					  + "You can choose to interact with the other villagers, do nothing and observe, or speak to the group (please do so in-character, and use natural language)."
		});
		var response = await chatGptApi.GetChatGptResponseAsync(messages.ToArray(),
						   actions.Select(a => new GptFunction {
							   Name = a.Name,
							   Description = a.Description,
							   Parameters = a.Parameters
						   }), ToolChoice.Required);

		logger.LogGptUsage(response.Usage);

		var calls = response.Choices.First().Message.ToolCalls;
		var details = new List<IActivityDetails>();
		foreach (var call in calls ?? []) {
			var action = actions.FirstOrDefault(a => a.Name == call.Function.Name);
			if (action == null) {
				logger.LogInvalidAction(villager, call.Function);
				continue;
			}

			details.Add(action.Execute(call.Function.Arguments, new VillagerActionState {
				World = world,
				Actor = villager,
				Others = villagers.Where(v => v != villager).ToArray()
			}));
		}

		villager.CurrentActivity = new Activity(details.First(), world);
		foreach (var activityDetail in details) {
			villager.ActivityQueue.Push(new Activity(activityDetail, world));
		}
	}
}