using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class AdjustEmotionalStateAction(
	EventsService eventService,
	VillagerEmotionService emotionService
) : IAction {
	public string Name => "AdjustEmotionalState";
	public ActivityName ActivityName => ActivityName.AdjustEmotionalState;
	public string Description => "Adjusts your emotional state by a specified amount.";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<AdjustEmotionalStateArguments>();

	public Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<AdjustEmotionalStateArguments>(arguments) ??
				   throw new NullReferenceException();
		return Task.FromResult<ActivityDto>(new AdjustEmotionalStateActivityDto {
			TotalDuration = TimeSpan.FromSeconds(2),
			Emotion = args.Emotion,
			Adjustment = args.Adjustment,
			Reason = args.Reason
		});
	}

	public Task<IActionResults> Begin(ActivityDto activityDto) => Task.FromResult<IActionResults>(new ActionResults());

	public async Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not AdjustEmotionalStateActivityDto args) {
			throw new ArgumentException("ActivityDto is not of type AdjustEmotionalStateActivityDto");
		}

		var adjustmentString = args.Adjustment > 0 ? $"+{args.Adjustment}" : $"{args.Adjustment}";
		var activity =
			$"{args.Villager.Name} {args.Reason} [{args.Emotion} {adjustmentString}% ({args.Villager.Emotions[args.Emotion] + args.Adjustment}%)]";
		await eventService.AddAsync(args.Villager, activity);
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
					 " It will automatically be prepended with your name, ie \"is comforted by John\" will become \"Marry is comforted by John\"." +
					 " DO NOT include your name at the start of the reason, it will be added automatically." +
					 " Make sure to pay close attention to everyone's gender, and to use the correct pronouns, especially your own." +
					 " Some examples are:" +
					 " \"is scared by the sudden appearance of monsters.\" +" +
					 " \"is happy to hear the news of a new baby.\"" +
					 " \"relaxes, and calms his nerves.\"")]
	public string Reason { get; set; } = string.Empty;
}