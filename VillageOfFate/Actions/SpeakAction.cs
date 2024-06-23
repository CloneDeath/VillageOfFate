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

public class SpeakAction(VillageLogger logger) : IAction {
	public string Name => "Speak";
	public ActivityName ActivityName => ActivityName.Speak;

	public string Description => "Say something";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<SpeakArguments>();

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
		var args = JsonSerializer.Deserialize<SpeakArguments>(arguments) ?? throw new NullReferenceException();
		var activity = $"[{state.World.CurrenTime}] {state.Actor.Name} says: \"{args.Content}\"";
		logger.LogActivity(activity);
		foreach (var v in state.Others.Append(state.Actor)) {
			v.AddMemory(activity);
		}

		return new ActivityDetails {
			Description = "Speaking",
			Duration = CalculateSpeakDuration(args.Content),
			Interruptible = false,
			OnCompletion = () => new ActivityResult { TriggerReactions = state.Others }
		};
	}

	public static TimeSpan CalculateSpeakDuration(string sentence) {
		const double averageSecondsPerWord = 0.45;
		var wordCount = sentence.Split(' ').Length;
		return TimeSpan.FromSeconds(wordCount * averageSecondsPerWord);
	}
}

public class SpeakArguments {
	[JsonRequired]
	[JsonPropertyName("content")]
	[JsonDescription("what to say")]
	public string Content { get; set; } = string.Empty;
}