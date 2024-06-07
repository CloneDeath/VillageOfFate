using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class AdjustEmotionalStateAction(
	TimeService time,
	VillagerMemoryService memoryService,
	VillagerEmotionService emotionService) : IAction {
	public string Name => "AdjustEmotionalState";
	public string Description => "Adjusts your emotional state by a specified amount.";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<AdjustEmotionalStateArguments>();

	public ActivityDto ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<AdjustEmotionalStateArguments>(arguments) ??
				   throw new NullReferenceException();
		return new AdjustEmotionalStateActivityDto {
			Description = "Adjusting Emotional State",
			Interruptible = true,
			Duration = TimeSpan.FromSeconds(2),
			Emotion = args.Emotion,
			Adjustment = args.Adjustment,
			Reason = args.Reason
		};
	}

	public Task<IActionResults> Begin(ActivityDto activityDto) => Task.FromResult<IActionResults>(new ActionResults());

	public async Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not AdjustEmotionalStateActivityDto args) {
			throw new ArgumentException("ActivityDto is not of type AdjustEmotionalStateActivityDto");
		}

		var adjustmentString = args.Adjustment > 0 ? $"+{args.Adjustment}" : $"{args.Adjustment}";
		var activity =
			$"[{time.GetAsync(TimeLabel.World)}] {args.Villager.Name} [{args.Emotion} {adjustmentString}% ({args.Villager.Emotions[args.Emotion]}%)]: {args.Reason}";
		await memoryService.AddAsync(args.Villager, activity);
		await emotionService.AdjustEmotionAsync(args.Villager, args.Emotion, args.Adjustment);
		return new ActionResults();
	}
}

public class AdjustEmotionalStateArguments {
	[JsonConverter(typeof(JsonStringEnumConverter))]
	[JsonPropertyName("emotion")]
	[JsonDescription("the emotion to adjust")]
	public VillagerEmotion Emotion { get; set; }

	[JsonPropertyName("adjustment")]
	[JsonDescription("the amount to adjust the emotion by. Can be positive or negative")]
	public int Adjustment { get; set; }

	[JsonPropertyName("reason")]
	[JsonDescription("A brief description of why your mood has changed." +
					 " Should usually start with \"I <verb>\" like \"I am comforted by X's presence\"," +
					 " but could also be stated like \"The sudden appearance of monsters scares me.\"")]
	public string Reason { get; set; } = string.Empty;
}