using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Legacy;
using VillageOfFate.Legacy.Activities;
using VillageOfFate.Legacy.VillagerActions;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class LookoutAction(VillageLogger logger) : IAction {
	public string Name => "Lookout";
	public ActivityName ActivityName => ActivityName.Lookout;

	public string Description => "Keep a Lookout for monsters";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<LookoutArguments>();

	public ActivityDto ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<LookoutArguments>(arguments)
				   ?? throw new NullReferenceException();
		return new LookoutActivityDto {
			Description = "Doing Nothing",
			Interruptible = true
		};
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) =>
		Task.FromResult<IActionResults>(new ActionResults());

	public Task<IActionResults> End(ActivityDto activityDto) => Task.FromResult<IActionResults>(new ActionResults());

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
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to keep a lookout in hours")]
	public double DurationInHours { get; set; }
}