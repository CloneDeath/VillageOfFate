using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using VillageOfFate.Activities;

namespace VillageOfFate.VillagerActions;

public class SleepAction(VillageLogger logger) : IVillagerAction {
	public string Name => "Sleep";
	public string Description => "Sleeps";
	public object Parameters => new {
		type = "object",
		properties = new {
			durationInHours = new {
				type = "number",
				description = "how long to sleep in hours"
			}
		},
		required = new[] { "durationInHours" }
	};

	public IActivityDetails Execute(string arguments, VillagerActionState state) {
		var args = JsonSerializer.Deserialize<SleepArguments>(arguments) ?? throw new NullReferenceException();
		var activity = $"[{state.World.CurrenTime}] {state.Actor.Name} lays down to rest";
		logger.LogActivity(activity);
		foreach (var v in state.Others.Append(state.Actor)) {
			v.AddMemory(activity);
		}

		return new ActivityDetails {
			Description = "Sleeping",
			Duration = TimeSpan.FromHours(args.DurationInHours),
			Interruptible = false,
			OnCompletion = () => {
				var completionActivity = $"[{state.World.CurrenTime}] {state.Actor.Name} wakes up from an {args.DurationInHours}-hour rest";
				logger.LogActivity(completionActivity);
				foreach (var v in state.Others.Append(state.Actor)) {
					v.AddMemory(completionActivity);
				}

				return new ActivityResult { TriggerReactions = [] };
			}
		};
	}
}

public class SleepArguments {
	[JsonPropertyName("durationInHours")] public double DurationInHours { get; set; }
}