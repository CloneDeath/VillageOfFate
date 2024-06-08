using System.Collections.Generic;
using System.Linq;
using VillageOfFate.DAL.Entities;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server;

public static class WebModelConversions {
	public static WebVillager AsWebVillager(this VillagerDto villager) =>
		new() {
			Id = villager.Id,
			Name = villager.Name,
			Gender = villager.Gender,
			Summary = villager.Summary,
			Age = villager.Age,
			//Emotions = AsWebVillagerEmotions(villager.Emotions),
			SectorLocation = new Position(villager.Sector.X, villager.Sector.Y),
			Hunger = villager.Hunger,
			Inventory = villager.Items.Select(AsWebItem).ToList(),
			CurrentActivity = villager.CurrentActivity?.AsWebActivity(),
			ActivityQueue = new Stack<WebActivity>(villager.ActivityQueue.Select(a => a.AsWebActivity()))
		};

	public static WebItem AsWebItem(this ItemDto item) => new() {
		Id = item.Id,
		Name = item.Name,
		Description = item.Description,
		Edible = item.Edible,
		HungerRestored = item.HungerRestored,
		Quantity = item.Quantity,
		Base64Image = item.Image?.Base64Image
	};

	public static WebVillagerEmotions AsWebVillagerEmotions(this EmotionDto emotions) =>
		new() {
			Happiness = emotions.Happiness,
			Sadness = emotions.Sadness,
			Fear = emotions.Fear
		};

	public static WebActivity AsWebActivity(this ActivityDto activity) =>
		new() {
			Description = activity.Description,
			Interruptible = activity.Interruptible,
			Duration = activity.Duration,
			StartTime = activity.StartTime,
			EndTime = activity.EndTime
		};

	public static WebSector AsWebSector(this SectorDto sector) =>
		new(sector.Position) {
			Id = sector.Id,
			Description = sector.Description,
			Items = sector.Items.Select(AsWebItem).ToList()
		};
}