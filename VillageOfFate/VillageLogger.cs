using System;
using GptApi;

namespace VillageOfFate;

public class VillageLogger {
	public virtual void LogActivity(string activity) {
		Console.WriteLine(activity);
	}

	public virtual void LogGptUsage(Usage usage) {
		Console.WriteLine($"GPT Cost: {usage.TotalTokens} Tokens ({usage.PromptTokens} Prompt Tokens, {usage.CompletionTokens} Completion Tokens)");
	}

	public virtual void LogInvalidAction(Villager villager, FunctionCall callFunction) {
		Console.WriteLine($"{villager.Name} tried to execute an Invalid Action '{callFunction.Name}' with args '{callFunction.Arguments}'");
	}
}