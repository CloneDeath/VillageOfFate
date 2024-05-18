using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VillageOfFate.Activities;

namespace VillageOfFate.VillagerActions;

public class AdjustEmotionalStateAction(VillageLogger logger) : IVillagerAction {
	public string Name => "AdjustEmotionalState";
	public string Description => "Adjusts your emotional state by a specified amount.";
	public object Parameters => new {
		type = "object",
		properties = new {
			emotion = new {
				type = "string",
				@enum = Enum.GetNames(typeof(VillagerEmotion)),
				description = "the emotion to adjust"
			},
			adjustment = new {
				type = "integer",
				description = "the amount to adjust the emotion by. Can be positive or negative"
			},
			reason = new {
				type = "string",
				description = "A brief description of why your mood has changed." +
							  " Should usually start with \"I <verb>\" like \"I am comforted by X's presence\"," +
							  " but could also be stated like \"The sudden appearance of monsters scares me.\""
			}
		},
		required = new[] { "emotion", "adjustment", "reason" }
	};

	public IActivityDetails Execute(string arguments, VillagerActionState state) {
		var args = JsonSerializer.Deserialize<AdjustEmotionalStateArguments>(arguments) ??
				   throw new NullReferenceException();

		return new ActivityDetails {
			Description = "Adjusting Emotional State",
			Duration = TimeSpan.FromSeconds(2),
			Interruptible = true,
			OnCompletion = () => {
				var adjustmentString = args.Adjustment > 0 ? $"+{args.Adjustment}" : $"{args.Adjustment}";
				var activity =
					$"[{state.World.CurrenTime}] {state.Actor.Name} [{args.Emotion} {adjustmentString}% ({state.Actor.Emotions[args.Emotion]}%)]: {args.Reason}";
				logger.LogActivity(activity);
				state.Actor.AddMemory(activity);

				state.Actor.AdjustEmotion(args.Emotion, args.Adjustment);
				return new ActivityResult { TriggerReactions = [] };
			}
		};
	}
}

public class AdjustEmotionalStateArguments {
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[JsonPropertyName("emotion")]
	public VillagerEmotion Emotion { get; set; }

	[JsonPropertyName("adjustment")] public int Adjustment { get; set; }

	[JsonPropertyName("reason")] public string Reason { get; set; } = string.Empty;
}