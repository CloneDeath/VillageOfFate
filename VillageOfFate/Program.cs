using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using GptApi;
using VillageOfFate.Activities;
using VillageOfFate.VillagerActions;

namespace VillageOfFate;

public class Program {
	public static async Task Main(string[] args) {
		var parser = new Parser(with => with.HelpWriter = Console.Error);
		var result = parser.ParseArguments<ProgramOptions>(args);

		var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
					 ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

		var chatGptApi = new ChatGptApi(apiKey) {
			Model = GptModel.Gpt_4_Omni
		};

		var random = new RandomProvider();
		var world = GetInitialWorld();
		var villagers = GetInitialVillagers(world, random);

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

		var endTime = world.CurrenTime + TimeSpan.FromMinutes(2);
		while (world.CurrenTime < endTime) {
			var villager = GetVillagerWithTheShortestCompleteTime(villagers);
			var waitTime = villager.CurrentActivity.EndTime - world.CurrenTime;
			if (villager.CurrentActivity.EndTime > world.CurrenTime) {
				// If the villager's current activity is not yet complete, wait until it is
				Thread.Sleep(waitTime);
				world.CurrenTime += waitTime;
			}

			var activityResult = villager.CurrentActivity.OnCompletion();
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
					$"- Hunger: {villager.Hunger} (+1 per hour)"
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

	private static World GetInitialWorld() {
		var world = new World();
		var sector = world.CreateSector(new Point(0, 0));
		sector.Description =
			"A dense, lush forest filled with towering trees, diverse wildlife, and the sounds of nature. " +
			"It's easy to lose one's way in this vast sea of green.";

		sector.Items.Add(new Item {
			Name = "Ration",
			Description = "A small bag of dried fruit and nuts.",
			Quantity = 3, Edible = true, HungerRestored = 2
		});
		sector.Items.AddRange(new List<Item> {
			new() {
				Name = "Apple",
				Description = "A juicy, red apple. Perfect for a quick snack.",
				Quantity = 1, Edible = true, HungerRestored = 5
			},
			new() {
				Name = "Mushroom",
				Description = "A common forest mushroom. Make sure it's not poisonous before eating!",
				Quantity = 1, Edible = true, HungerRestored = 3
			},
			new() {
				Name = "Berries",
				Description = "A handful of wild berries. Sweet and nutritious.",
				Quantity = 1, Edible = true, HungerRestored = 4
			}
		});
		return world;
	}

	private static Villager[] GetInitialVillagers(World world, RandomProvider random) {
		var maxIdle = TimeSpan.FromSeconds(5);
		var gamz = new Villager {
			Name = "Gamz", Age = 26, Gender = Gender.Male,
			Summary = "Chemm's big brother. A warrior monk with multiple wounds on both his face and body.",
			SectorLocation = new Point(0, 0),
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		};
		var chem = new Villager {
			Name = "Chemm", Age = 19, Gender = Gender.Female,
			Summary = "Gamz's little sister. A priestess who believes in the god of fate.",
			SectorLocation = new Point(0, 0),
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		};
		var carol = new Villager {
			Name = "Carol", Age = 7, Gender = Gender.Female,
			Summary = "A cheerful child, although quite mature for her age.",
			SectorLocation = new Point(0, 0),
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		};
		var lyra = new Villager {
			Name = "Lyra", Age = 30, Gender = Gender.Female,
			Summary = "A younger wife than her husband, but capable of keeping him in check.",
			SectorLocation = new Point(0, 0),
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		};
		var lodis = new Villager {
			Name = "Lodis", Age = 33, Gender = Gender.Male,
			Summary = "The father of a family of three that ran a general store in the village.",
			SectorLocation = new Point(0, 0),
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		};

		gamz.IncreaseHunger(6);
		gamz.AddRelationship(chem, "Younger Sister");
		gamz.AddRelationship(carol, "Child of Neighbors");
		gamz.AddRelationship(lyra, "Neighbor");
		gamz.AddRelationship(lodis, "Neighbor");

		chem.IncreaseHunger(5);
		chem.AddRelationship(gamz, "Older Brother");
		chem.AddRelationship(carol, "Friend");
		chem.AddRelationship(lyra, "Neighbor");
		chem.AddRelationship(lodis, "Neighbor");

		carol.IncreaseHunger(8);
		carol.AddRelationship(gamz, "Neighbor");
		carol.AddRelationship(chem, "Friend");
		carol.AddRelationship(lyra, "Mom");
		carol.AddRelationship(lodis, "Dad");

		lyra.IncreaseHunger(4);
		lyra.AddRelationship(gamz, "Neighbor");
		lyra.AddRelationship(chem, "Neighbor");
		lyra.AddRelationship(carol, "Daughter");
		lyra.AddRelationship(lodis, "Husband");

		lodis.IncreaseHunger(5);
		lodis.AddRelationship(gamz, "Neighbor");
		lodis.AddRelationship(chem, "Neighbor");
		lodis.AddRelationship(carol, "Daughter");
		lodis.AddRelationship(lyra, "Wife");

		var villagers = new[] { gamz, chem, carol, lyra, lodis };
		foreach (var villager in villagers) {
			villager.AddMemory($"You and {villagers.Length - 1} other villagers are lost in the woods, "
							   + "having just escaped a goblin attack that destroyed your home and entire village.");
			world.AddVillager(villager);
		}

		return villagers;
	}
}