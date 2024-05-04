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

		var villagers = new[] {
			new Villager {
				Name = "Gamz", Age = 26, Gender = Gender.Male,
				Summary = "Chemm's big brother. A warrior monk with multiple wounds on both his face and body."
			},
			new Villager {
				Name = "Chemm", Age = 19, Gender = Gender.Female,
				Summary = "Gamz's little sister. A priest who believes in the god of fate."
			},
			new Villager {
				Name = "Carol", Age = 7, Gender = Gender.Female,
				Summary = "A cheerful child, although quite mature for her age."
			},
			new Villager {
				Name = "Lyra", Age = 30, Gender = Gender.Female,
				Summary = "A younger wife than his husband, but capable of keeping him in check."
			},
			new Villager {
				Name = "Lodis", Age = 33, Gender = Gender.Male,
				Summary = "The father of a family of three that ran a general store in the village."
			}
		};

		var history = new List<string>();

		foreach (var villager in villagers) {
			var messages = new List<Message> {
				new() {
					Role = Role.System,
					Content = $"Respond as {villager.Name} would. {villager.GetDescription()}"
				},
				new() {
					Role = Role.User,
					Content = "5 villagers are in the woods, having just been attacked by goblins.\n"
							  + villagers.Skip(1).Select(v => v.GetDescription()).Aggregate((a, b) => a + "\n" + b)
				},
			};
			messages.AddRange(history.Select(h => new Message {
				Role = Role.User,
				Content = h
			}));
			messages.Add(new Message { Role = Role.User, Content = "Please choose an action befitting your character." });
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
}