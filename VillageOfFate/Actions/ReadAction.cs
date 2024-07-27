using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

[RegisterAction]
public class ReadAction(ItemService items, EventsService events) : IAction {
	public string Name => "Read";
	public ActivityName ActivityName => ActivityName.Read;
	public string Description => "Read a page from a book";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<ReadArguments>();

	public async Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<ReadArguments>(arguments)
				   ?? throw new NullReferenceException();
		var page = await FindPage(args);

		return new ReadActivityDto {
			ReadingMode = args.ReadingMode,
			TargetItem = page,
			TotalDuration = CalculateReadDuration(page.Content, args.ReadingMode)
		};
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		if (activityDto is not ReadActivityDto readActivity) {
			throw new ArgumentException("ActivityDto is not a ReadActivityDto");
		}

		var villager = activityDto.Villager;
		switch (readActivity.ReadingMode) {
			case ReadingMode.ReadAloud: {
				var activity = $"{villager.Name} reads aloud \"{readActivity.TargetItem.Content}\"";
				await events.AddAsync(villager, villager.Sector.Villagers, activity);
				return new ActionResults();
			}
			case ReadingMode.SilentReading: {
				var activity = $"{villager.Name} reads to themselves \"{readActivity.TargetItem.Content}\"";
				await events.AddAsync(villager, activity);
				return new ActionResults();
			}
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not ReadActivityDto readActivity) {
			throw new ArgumentException("ActivityDto is not a SpeakActivityDto");
		}

		if (readActivity.ReadingMode == ReadingMode.ReadAloud) {
			var villager = readActivity.Villager;
			var others = villager.Sector.Villagers.Where(v => v.Id != villager.Id).ToList();
			return Task.FromResult<IActionResults>(new ActionResults { TriggerReactions = others });
		}

		return Task.FromResult<IActionResults>(new ActionResults());
	}

	private async Task<ItemDto> FindPage(ReadArguments args) {
		var item = await items.GetAsync(args.TargetItemId);
		if (item.Definition.Category == ItemCategory.Page) {
			return item;
		}

		if (args.PageNumber == null) {
			throw new ArgumentException("Page number is required when reading a book");
		}

		return await items.GetChildItemPageAsync(item.Id, args.PageNumber.Value);
	}

	public static TimeSpan CalculateReadDuration(string sentence, ReadingMode mode) {
		switch (mode) {
			case ReadingMode.ReadAloud:
				return SpeakAction.CalculateSpeakDuration(sentence);
			case ReadingMode.SilentReading: {
				const double averageSecondsPerWord = 0.25;
				var wordCount = sentence.Split(' ').Length;
				return TimeSpan.FromSeconds(wordCount * averageSecondsPerWord);
			}
			default:
				throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
		}
	}
}

public class ReadArguments {
	[JsonRequired]
	[JsonPropertyName("targetItemId")]
	[JsonDescription("The Item.Id of either a book or a page")]
	public Guid TargetItemId { get; set; }

	[JsonRequired]
	[JsonPropertyName("readingMode")]
	[JsonDescription("User can choose to read silently (value:'SilentReading') or read aloud (value:'ReadAloud')")]
	public ReadingMode ReadingMode { get; set; }

	[JsonPropertyName("pageNumber")]
	[JsonDescription("The page number to read; use -1 to read the last page. " +
					 "Only needed if reading a book, otherwise, omit when reading a page.")]
	public int? PageNumber { get; set; }
}