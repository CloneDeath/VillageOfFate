using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GptApi;

namespace VillageOfFate;

public class Program {
	public static async Task Main() {
		var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
			?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

		var chatGptApi = new ChatGptApi(apiKey);

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
					+ "# Relationships\n"
					+ string.Join("\n", villager.GetRelationships().Select(r => $"- {r.Villager.Name}: {r.Relation}"))
				}
			};
			messages.AddRange(villager.GetMemory().Select(h => new Message {
				Role = Role.User,
				Content = h
			}));
			messages.Add(new Message {
				Role = Role.User,
				Content = "Please choose an action befitting your character."
					+ "Keep your gender, age, role, history, and personality in mind."
					+ "Act like a real person in a fantasy world. Don't declare your actions, just do them."
					+ "You can choose to interact with the other villagers, do nothing and observe, or speak to the group (please do so in-character, and use natural language)."
			});
			var response = await chatGptApi.GetChatGptResponseAsync(messages.ToArray(), new []{
            	new GptFunction {
            		Name = "Speak",
            		Description = "Say something",
            		Parameters = new {
            			type = "object",
            			properties = new {
            				content = new {
            					type = "string",
            					description = "what to say"
            				}
            			}
            		}
            	},
            	new GptFunction {
            		Name = "DoNothing",
            		Description = "Do nothing, and observe"
            	},
            	new GptFunction {
            		Name = "Interact",
            		Description = "Interact with someone else",
            		Parameters = new {
            			type = "object",
            			properties = new {
            				targets = new {
            					type = "array",
            					items = new {
            						type = "string"
            					},
            					description = "who the action is directed at"
            				},
            				action = new {
            					type = "string",
            					description = "A description of the interaction you are performing"
            				}
            			}
            		}
            	}
            }, ToolChoice.Required);
            var calls = response.Choices.First().Message.ToolCalls;
            foreach (var call in calls ?? []) {
            	var activity = $"{villager.Name}: {call.Function.Name} {call.Function.Arguments}";
				Console.WriteLine(activity);
				history.Add(activity);
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
			Summary = "Gamz's little sister. A priest who believes in the god of fate."
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