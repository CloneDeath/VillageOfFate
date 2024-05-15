using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GptApi;
using VillageOfFate.VillagerActions;

namespace VillageOfFate;

public class Program {
	public static async Task Main() {
		var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
			?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

		var chatGptApi = new ChatGptApi(apiKey) {
			Model = GptModel.Gpt_4_Omni
		};

		var villagers = GetInitialVillagers();

		foreach (var villager in villagers) {
			villager.AddMemory($"You and {villagers.Length - 1} other villagers are lost in the woods, "
							   + "having just escaped a goblin attack that destroyed your home and entire village.");

		}

		foreach (var villager in villagers) {
			var messages = new List<Message> {
				new() {
					Role = Role.System,
					Content = $"Respond as {villager.Name} would. {villager.GetDescription()}\n"
							  + "Keep your gender, age, role, history, and personality in mind."
							  + "Act like a real person in a fantasy world. Don't declare your actions, just do them."
							  + "# Relationships\n"
							  + string.Join("\n",
								  villager.GetRelationships().Select(r =>
									  $"- {r.Villager.Name}: {r.Villager.GetDescription()} Relation: {r.Relation}"))
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
			List<IVillagerAction> actions = [
				new SpeakAction(),
				new DoNothingAction(),
				new InteractAction()
			];
			var response = await chatGptApi.GetChatGptResponseAsync(messages.ToArray(),
			   actions.Select(a => new GptFunction {
				   Name = a.Name,
				   Description = a.Description,
				   Parameters = a.Parameters
			   }), ToolChoice.Required);

			Console.WriteLine($"GPT Cost: {response.Usage.TotalTokens} Tokens ({response.Usage.PromptTokens} Prompt Tokens, {response.Usage.CompletionTokens} Completion Tokens)");

            var calls = response.Choices.First().Message.ToolCalls;
            foreach (var call in calls ?? []) {
				var action = actions.FirstOrDefault(a => a.Name == call.Function.Name);
				if (action == null) {
					Console.WriteLine($"Invalid Action: {call.Function.Name}");
                    continue;
				}

				action.Execute(call.Function.Arguments, new VillagerActionState {
					Actor = villager,
					Others = villagers.Where(v => v != villager).ToArray()
				});
			}
		}

		Console.ReadLine();
	}

	private static Villager[] GetInitialVillagers() {
		var gamz = new Villager {
			Name = "Gamz", Age = 26, Gender = Gender.Male,
			Summary = "Chemm's big brother. A warrior monk with multiple wounds on both his face and body."
		};
		var chem = new Villager {
			Name = "Chemm", Age = 19, Gender = Gender.Female,
			Summary = "Gamz's little sister. A priestess who believes in the god of fate."
		};
		var carol = new Villager {
			Name = "Carol", Age = 7, Gender = Gender.Female,
			Summary = "A cheerful child, although quite mature for her age."
		};
		var lyra = new Villager {
			Name = "Lyra", Age = 30, Gender = Gender.Female,
			Summary = "A younger wife than his husband, but capable of keeping him in check."
		};
		var lodis = new Villager {
			Name = "Lodis", Age = 33, Gender = Gender.Male,
			Summary = "The father of a family of three that ran a general store in the village."
		};

		gamz.AddRelationship(chem, "Younger Sister");
		gamz.AddRelationship(carol, "Child of Neighbors");
		gamz.AddRelationship(lyra, "Neighbor");
		gamz.AddRelationship(lodis, "Neighbor");

		chem.AddRelationship(gamz, "Older Brother");
		chem.AddRelationship(carol, "Friend");
		chem.AddRelationship(lyra, "Neighbor");
		chem.AddRelationship(lodis, "Neighbor");

		carol.AddRelationship(gamz, "Neighbor");
		carol.AddRelationship(chem, "Friend");
		carol.AddRelationship(lyra, "Mom");
		carol.AddRelationship(lodis, "Dad");

		lyra.AddRelationship(gamz, "Neighbor");
		lyra.AddRelationship(chem, "Neighbor");
		lyra.AddRelationship(carol, "Daughter");
		lyra.AddRelationship(lodis, "Husband");

		lodis.AddRelationship(gamz, "Neighbor");
		lodis.AddRelationship(chem, "Neighbor");
		lodis.AddRelationship(carol, "Daughter");
		lodis.AddRelationship(lyra, "Wife");

		return [gamz, chem, carol, lyra, lodis];
	}
}