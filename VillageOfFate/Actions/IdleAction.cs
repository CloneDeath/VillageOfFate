using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;

namespace VillageOfFate.Actions;

public class IdleAction : IAction {
	public string Name => "DoNothing";
	public string Description => "You will do nothing, sit idle, and wait for something to happen.";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<IdleArguments>();

	public ActivityDto ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<IdleArguments>(arguments)
				   ?? throw new NullReferenceException();
		return new IdleActivityDto {
			Description = "Doing Nothing",
			Interruptible = true,
			Duration = TimeSpan.FromHours(args.DurationInHours)
		};
	}

	public IActionResults Begin(ActivityDto activityDto) => new ActionResults();

	public IActionResults End(ActivityDto activityDto) => new ActionResults();
}

public class IdleArguments {
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to stay idle")]
	public double DurationInHours { get; set; }
}