using System;
using System.Text.Json.Serialization;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Legacy.Activities;
using VillageOfFate.Legacy.VillagerActions;

namespace VillageOfFate.Actions;

public class IdleAction : IAction {
	public string Name => "DoNothing";
	public string Description => "You will do nothing, sit idle, and wait for something to happen.";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<IdleArguments>();

	public ActivityDto ParseArguments(string arguments) => throw new NotImplementedException();

	public IActionResults Begin(ActivityDto activityDto) => throw new NotImplementedException();

	public IActionResults End(ActivityDto activityDto) => throw new NotImplementedException();

	public IActivityDetails Execute(string arguments, VillagerActionState state) =>
		new ActivityDetails {
			Description = "Doing Nothing",
			Interruptible = true,
			Duration = TimeSpan.FromSeconds(10)
		};
}

public class IdleArguments {
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to stay idle")]
	public double DurationInHours { get; set; }
}