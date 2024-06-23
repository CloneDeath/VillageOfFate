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

public class EatAction(VillageLogger logger) : IAction {
	public string Name => "Eat";
	public ActivityName ActivityName => ActivityName.Eat;

	public string Description => "Eat some Food";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<EatArguments>();

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
		var args = JsonSerializer.Deserialize<EatArguments>(arguments) ?? throw new NullReferenceException();
		var sector = state.World.GetSector(state.Actor.SectorLocation);
		var target = sector.Items.First(i => i.Id == args.TargetItemId);
		if (!target.Edible) {
			throw new Exception($"Item {target.Name} is not Edible!");
		}

		if (target.Quantity > 1) {
			target.Quantity -= 1;
		} else {
			sector.Items.Remove(target);
		}

		var activity = $"[{state.World.CurrenTime}] {state.Actor.Name} starts to eat {GetNameWithArticle(target.Name)}";
		logger.LogActivity(activity);
		var villagersInSector = state.World.GetVillagersInSector(state.Actor.SectorLocation).ToList();
		foreach (var v in villagersInSector) {
			v.AddMemory(activity);
		}

		return new ActivityDetails {
			Description = "Eating",
			Duration = TimeSpan.FromMinutes(7) * target.HungerRestored,
			Interruptible = true,
			OnCompletion = () => {
				state.Actor.DecreaseHunger(target.HungerRestored);

				var completionActivity =
					$"[{state.World.CurrenTime}] {state.Actor.Name} finishes eating {GetNameWithArticle(target.Name)} (Hunger -{target.HungerRestored})";
				logger.LogActivity(completionActivity);
				foreach (var v in villagersInSector) {
					v.AddMemory(completionActivity);
				}

				return new ActivityResult { TriggerReactions = state.Others };
			}
		};
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