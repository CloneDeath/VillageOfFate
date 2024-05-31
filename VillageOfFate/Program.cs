using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using GptApi;
using VillageOfFate.Activities;
using VillageOfFate.VillagerActions;

namespace VillageOfFate;

public static class Program {
	public static async Task Main(string[] args) {
		var parser = new Parser(with => with.HelpWriter = Console.Error);
		var result = parser.ParseArguments<ProgramOptions>(args);

		// Moving to Server...

		var logger = new VillageLogger(result.Value.LogDirectory ?? Directory.GetCurrentDirectory());
		List<IVillagerAction> actions = [
			new SpeakAction(logger),
			new DoNothingAction(),
			new InteractAction(logger),
			new AdjustEmotionalStateAction(logger),
			new EatAction(logger),
			new SleepAction(logger),
			new LookoutAction(logger)
		];

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

	private static Villager GetVillagerWithTheShortestCompleteTime(Villager[] villagers) {
		return villagers.OrderBy(v => v.CurrentActivity.StartTime + v.CurrentActivity.Duration).First();
	}
}