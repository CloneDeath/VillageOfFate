using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

[RegisterAction]
public class SpeakAction(EventsService events) : IAction {
	public string Name => "Speak";
	public ActivityName ActivityName => ActivityName.Speak;

	public string Description => "Say something";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<SpeakArguments>();

	public Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<SpeakArguments>(arguments)
				   ?? throw new NullReferenceException();
		return Task.FromResult<ActivityDto>(new SpeakActivityDto {
			TotalDuration = CalculateSpeakDuration(args.Content),
			Content = args.Content
		});
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		if (activityDto is not SpeakActivityDto speakActivity) {
			throw new ArgumentException("ActivityDto is not a SpeakActivityDto");
		}

		var villager = activityDto.Villager;
		var activity = $"{villager.Name} says: \"{speakActivity.Content}\"";
		await events.AddAsync(villager, villager.Sector.Villagers, activity);
		return new ActionResults();
	}

	public Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not SpeakActivityDto speakActivity) {
			throw new ArgumentException("ActivityDto is not a SpeakActivityDto");
		}

		var villager = speakActivity.Villager;
		var others = villager.Sector.Villagers.Where(v => v.Id != villager.Id).ToList();
		return Task.FromResult<IActionResults>(new ActionResults { TriggerReactions = others });
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