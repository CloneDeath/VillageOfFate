using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Legacy;
using VillageOfFate.Legacy.Activities;
using VillageOfFate.Legacy.VillagerActions;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class SleepAction(VillageLogger logger) : IAction {
	public string Name => "Sleep";
	public ActivityName ActivityName => ActivityName.Sleep;

	public string Description => "Sleep for a while";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<SleepArguments>();

	public ActivityDto ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<>(arguments)
				   ?? throw new NullReferenceException();
		return new {
			Description = "Doing Nothing",
			Interruptible = true
		};
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) =>
		Task.FromResult<IActionResults>(new ActionResults());

	public Task<IActionResults> End(ActivityDto activityDto) => Task.FromResult<IActionResults>(new ActionResults());

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
				var completionActivity =
					$"[{state.World.CurrenTime}] {state.Actor.Name} wakes up from an {args.DurationInHours}-hour rest";
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
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to sleep in hours")]
	public double DurationInHours { get; set; }
}