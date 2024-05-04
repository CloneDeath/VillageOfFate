using System;
using System.Linq;
using System.Threading.Tasks;
using GptApi;

namespace VillageOfFate;

public class Program {
	public static async Task Main() {
		var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
					 ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");

		var chatGptApi = new ChatGptApi(apiKey);

		var response = await chatGptApi.GetChatGptResponseAsync(new[] {
			new Message {
				Role = Role.User,
				Content = "How are you today?"
			}
		});
		Console.WriteLine("Hello, World! " + response.Choices.First().Message.Content);
		Console.ReadLine();
	}
}