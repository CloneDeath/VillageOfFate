using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GptApi;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Legacy;
using VillageOfFate.Legacy.Activities;
using VillageOfFate.Legacy.VillagerActions;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate;

public class WorldRunner(
	TimeService time,
	VillagerService villagers,
	RelationshipService relationships,
	VillagerActivityService villagerActivities,
	ActivityFactory activityFactory,
	GptUsageService gptUsage,
	ChatGptApi chatGptApi,
	RandomProvider random) {
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
		if (villager.CurrentActivity == null) return;
		if (villager.CurrentActivity.EndTime > currentTime) return;

		var currentActivity = activityFactory.Get(villager.CurrentActivity);
		var activityResult = currentActivity.OnCompletion();

		if (villager.ActivityQueue.Any()) {
			await villagerActivities.PopAsync(villager);
		} else {
			await QueueActionsForVillager(villager);
			PushCurrentActivityIntoQueue(villager);
			villager.CurrentActivity = new IdleActivity(random.NextTimeSpan(TimeSpan.FromMinutes(2)), world);
		}

		if (activityResult.TriggerReactions.Any()) {
			var selected = random.SelectOne(activityResult.TriggerReactions);
			PushCurrentActivityIntoQueue(selected);
			await QueueActionsForVillager(selected);
		}
	}

	private static void PushCurrentActivityIntoQueue(Villager villager, World world) {
		var current = villager.CurrentActivity;
		var remainingTime = current.EndTime - world.CurrenTime;
		current.Duration = remainingTime < TimeSpan.Zero ? TimeSpan.Zero : remainingTime;
		villager.ActivityQueue.Push(current);
	}

	private async Task QueueActionsForVillager(VillagerDto villager) {
		var relations = relationships.Get(villager);
		var messages = new List<Message> {
			new() {
				Role = Role.System,
				Content = string.Join("\n", [
					$"Respond as {villager.Name} would. {villager.GetDescription()}",
					"Keep your gender, age, role, history, and personality in mind.",
					"Act like a real person in a fantasy world. Don't declare your actions, just do them.",
					"# Relationships",
					string.Join("\n",
						relations.Select(r =>
							$"- {r.Relation.Name}: {r.Relation.GetDescription()} Relation: {r.Relation}")),
					"# Emotions (0% = neutral, 100% = maximum intensity)",
					string.Join("\n", villager.Emotions.GetEmotions().Select(e => $"- {e.Emotion}: {e.Intensity}%")),
					"# Location",
					$"You are located at Sector Coordinate {villager.Sector.Position}.",
					$"Description: {villager.Sector.Description}",
					"Items:",
					string.Join("\n",
						villager.Sector.Items.Select(i => $"- {i.GetSummary()}")),
					"# Status",
					$"- Hunger: {villager.Hunger} (+1 per hour)",
					"# Inventory",
					string.Join("\n", villager.Items.Select(i => $"- {i.GetSummary()}"))
				])
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
						   activityFactory.Actions.Select(a => new GptFunction {
							   Name = a.Name,
							   Description = a.Description,
							   Parameters = a.Parameters
						   }), ToolChoice.Required);
		await gptUsage.AddUsageAsync(response);

		var calls = response.Choices.First().Message.ToolCalls;
		var details = new List<IActivityDetails>();
		foreach (var call in calls ?? []) {
			var action = activityFactory.Actions.FirstOrDefault(a => a.Name == call.Function.Name);
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