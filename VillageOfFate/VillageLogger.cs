using System;
using System.IO;
using GptApi;

namespace VillageOfFate;

public class VillageLogger {
	private readonly string _logFile;
	public VillageLogger() {
		var now = DateTime.Now;
		_logFile = $"{now.Year}-{now.Month:00}-{now.Day:00}_{now.Hour:00}-{now.Minute:00}-{now.Second:00}.txt";
	}

	public virtual void LogActivity(string activity) {
		Console.WriteLine(activity);
		File.AppendAllLines(_logFile, [$"{activity}"]);
	}

	public virtual void LogGptUsage(Usage usage) {
		var usageDescription = $"GPT Usage: {usage.TotalTokens} Tokens ({usage.PromptTokens} Prompt Tokens, {usage.CompletionTokens} Completion Tokens)";
		Console.WriteLine(usageDescription);
		File.AppendAllLines(_logFile, [$"{usageDescription}"]);

	}

	public virtual void LogInvalidAction(Villager villager, FunctionCall callFunction) {
		var invalidAction = $"{villager.Name} tried to execute an Invalid Action '{callFunction.Name}' with args '{callFunction.Arguments}'";
		Console.WriteLine(invalidAction);
		File.AppendAllLines(_logFile, [$"{invalidAction}"]);
	}
}