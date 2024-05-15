using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VillageOfFate.VillagerActions;

public class InteractAction(VillageLogger logger) : IVillagerAction {
	public string Name => "Interact";
	public string Description => "Interact with someone else";
	public object Parameters => new {
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
				description = "A description of the interaction you are performing." +
							  " Should usually start with \"I <verb>\" like \"I hold the apply\" or \"I reach out to the frog\""
			}
		},
		required = new[]{"targets", "action"}
	};

	public void Execute(string arguments, VillagerActionState state) {
		var args = JsonSerializer.Deserialize<InteractArguments>(arguments) ?? throw new NullReferenceException();
		var targets = state.Others.Where(o => args.Targets.Contains(o.Name));

		var targetNames = joinNames(args.Targets);
		var activity = $"{state.Actor.Name} interacts with {targetNames}: \"{args.Action}\"";
		logger.LogActivity(activity);
		foreach (var villager in targets.Append(state.Actor)) {
			villager.AddMemory(activity);
		}
	}

	private static string joinNames(IReadOnlyList<string> names) {
		switch (names.Count) {
			case 0: return "No one";
			case 1: return names[0];
		}

		var last = names[^1];
		var others = names.SkipLast(1);
		return $"{string.Join(", ", others)}, and {last}";
	}
}

public class InteractArguments {
	[JsonPropertyName("targets")] public string[] Targets { get; set; } = [];
	[JsonPropertyName("action")] public string Action { get; set; } = string.Empty;
}