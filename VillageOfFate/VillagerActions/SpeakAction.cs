using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VillageOfFate.VillagerActions;

public class SpeakAction(VillageLogger logger) : IVillagerAction {
	public string Name => "Speak";
	public string Description => "Say something";
	public object Parameters => new {
		type = "object",
		properties = new {
			content = new {
				type = "string",
				description = "what to say"
			}
		},
		required = new[]{"content"}
	};

	public void Execute(string arguments, VillagerActionState state) {
		var args = JsonSerializer.Deserialize<SpeakArguments>(arguments) ?? throw new NullReferenceException();
		var activity = $"[{state.World.CurrenTime}] {state.Actor.Name} says: \"{args.Content}\"";
		logger.LogActivity(activity);
		foreach (var v in state.Others.Append(state.Actor)) {
			v.AddMemory(activity);
		}
	}
}

public class SpeakArguments {
	[JsonPropertyName("content")] public string Content { get; set; } = string.Empty;
}