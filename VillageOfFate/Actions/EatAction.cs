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

public class EatAction(ItemService items, EventsService events, VillagerService villagers) : IAction {
	public string Name => "Eat";
	public ActivityName ActivityName => ActivityName.Eat;

	public string Description => "Eat some Food";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<EatArguments>();

	public async Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<EatArguments>(arguments)
				   ?? throw new NullReferenceException();
		var item = await items.GetAsync(args.TargetItemId);
		return new EatActivityDto {
			TotalDuration = TimeSpan.FromMinutes(7) * item.HungerRestored,
			TargetItem = item
		};
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		if (activityDto is not EatActivityDto eatActivity) {
			throw new Exception($"{nameof(ActivityDto)} is not {nameof(EatActivityDto)}");
		}

		var item = eatActivity.TargetItem;
		if (!item.Edible) {
			throw new Exception($"Item {item.Name} is not Edible!");
		}

		var villager = eatActivity.Villager;
		await items.ConsumeSingle(villager, item);

		var activity = $"{villager.Name} starts to eat {GetNameWithArticle(item.Name)}";
		await events.AddAsync(villager, villager.Sector.Villagers, activity);

		return new ActionResults();
	}

	public async Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not EatActivityDto eatActivity) {
			throw new Exception($"{nameof(ActivityDto)} is not {nameof(EatActivityDto)}");
		}

		var villager = eatActivity.Villager;
		var item = eatActivity.TargetItem;

		await villagers.DecreaseHungerAsync(villager, item.HungerRestored);

		var completionActivity =
			$"{villager.Name} finishes eating {GetNameWithArticle(item.Name)} (Hunger -{item.HungerRestored})";
		await events.AddAsync(villager, villager.Sector.Villagers, completionActivity);

		return new ActionResults();
	}

	public static string GetNameWithArticle(string itemName) {
		if (string.IsNullOrEmpty(itemName)) return itemName;

		var firstLetter = itemName[0];
		// ReSharper disable once StringLiteralTypo
		var isVowel = "aeiouAEIOU".Contains(firstLetter);
		return isVowel ? $"an {itemName}" : $"a {itemName}";
	}
}

public class EatArguments {
	[JsonRequired]
	[JsonPropertyName("targetItemId")]
	[JsonDescription("the Id of the item to be eaten")]
	public Guid TargetItemId { get; set; } = Guid.Empty;
}