using System;
using System.Collections.Generic;
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

public class InteractAction(VillageLogger logger) : IAction {
	public string Name => "Interact";
	public ActivityName ActivityName => ActivityName.Interact;

	public string Description => "Interact with someone else";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<InteractArguments>();

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
		var args = JsonSerializer.Deserialize<InteractArguments>(arguments) ?? throw new NullReferenceException();
		var targets = state.Others.Where(o => args.Targets.Contains(o.Name)).ToList();

		var targetNames = joinNames(args.Targets);
		var activity = $"[{state.World.CurrenTime}] {state.Actor.Name} interacts with {targetNames}: \"{args.Action}\"";
		logger.LogActivity(activity);
		foreach (var villager in targets.Append(state.Actor)) {
			villager.AddMemory(activity);
		}

		return new ActivityDetails {
			Description = "Interacting",
			Duration = TimeSpan.FromSeconds(args.DurationInSeconds),
			Interruptible = true,
			OnCompletion = () => new ActivityResult {
				TriggerReactions = targets
			}
		};
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
	[JsonRequired]
	[JsonPropertyName("targets")]
	[JsonDescription("who the action is directed at")]
	public string[] Targets { get; set; } = [];

	[JsonRequired]
	[JsonPropertyName("durationInSeconds")]
	[JsonDescription("The number of seconds the interaction takes")]
	public double DurationInSeconds { get; set; }

	[JsonRequired]
	[JsonPropertyName("action")]
	[JsonDescription("A description of the interaction you are performing." +
					 " It will automatically be prepended with your name, ie \"holds John's hand.\" will become \"Marry holds John's hand.\"." +
					 " DO NOT include your name at the start of the action, it will be added automatically." +
					 " Make sure to pay close attention to everyone's gender, and to use the correct pronouns, especially your own." +
					 " Some examples are:" +
					 " \"scares Jannet.\" +" +
					 " \"comforts the crying child.\"" +
					 " \"offers some food to Brian.\"")]
	public string Action { get; set; } = string.Empty;
}