using System.Collections.Generic;
using System.Linq;
using VillageOfFate.Legacy;
using VillageOfFate.Legacy.Activities;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server;

public static class WebModelConversions {
	public static WebVillager AsWebVillager(this Villager villager) =>
		new() {
			Id = villager.Id,
			Name = villager.Name,
			Gender = villager.Gender,
			Summary = villager.Summary,
			Age = villager.Age,
			Emotions = AsWebVillagerEmotions(villager.Emotions),
			SectorLocation = villager.SectorLocation,
			Hunger = villager.Hunger,
			Inventory = villager.Inventory,
			CurrentActivity = villager.CurrentActivity.AsWebActivity(),
			ActivityQueue = new Stack<WebActivity>(villager.ActivityQueue.Select(a => a.AsWebActivity()))
		};

	public static WebVillagerEmotions AsWebVillagerEmotions(this VillagerEmotions emotions) =>
		new() {
			Happiness = emotions.Happiness,
			Sadness = emotions.Sadness,
			Fear = emotions.Fear
		};

	public static WebActivity AsWebActivity(this Activity activity) =>
		new() {
			Description = activity.Description,
			Interruptible = activity.Interruptible,
			Duration = activity.Duration,
			StartTime = activity.StartTime,
			EndTime = activity.EndTime
		};

	public static WebWorld AsWebWorld(this World world) =>
		new() {
			Sectors = world.Sectors,
			CurrenTime = world.CurrenTime,
			Villagers = world.Villagers.Select(v => v.AsWebVillager())
		};
}