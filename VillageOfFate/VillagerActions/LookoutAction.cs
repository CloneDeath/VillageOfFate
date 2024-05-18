using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using VillageOfFate.Activities;

namespace VillageOfFate.VillagerActions;

public class LookoutAction(VillageLogger logger) : IVillagerAction {
	public string Name => "Lookout";
	public string Description => "On Lookout";
	public object Parameters => new {
		type = "object",
		properties = new {
			durationInHours = new {
				type = "number",
				description = "how long to stay on lookout"
			}
		},
		required = new[] { "durationInHours" }
	};

	public IActivityDetails Execute(string arguments, VillagerActionState state) {
		var args = JsonSerializer.Deserialize<LookoutArguments>(arguments) ?? throw new NullReferenceException();
		var activity = $"[{state.World.CurrenTime}] {state.Actor.Name} starts to lookout for monsters";
		logger.LogActivity(activity);
		foreach (var v in state.Others.Append(state.Actor)) {
			v.AddMemory(activity);
		}

		return new ActivityDetails {
			Description = "On Lookout",
			Duration = TimeSpan.FromHours(args.DurationInHours),
			Interruptible = true,
			OnCompletion = () => {
				var completionActivity = $"[{state.World.CurrenTime}] {state.Actor.Name} finishes their lookout duty";
				logger.LogActivity(completionActivity);
				foreach (var v in state.Others.Append(state.Actor)) {
					v.AddMemory(completionActivity);
				}

				return new ActivityResult { TriggerReactions = [] };
			}
		};
	}
}

public class LookoutArguments {
	[JsonPropertyName("durationInHours")] public double DurationInHours { get; set; }
}