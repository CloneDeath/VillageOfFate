using System;
using System.Collections.Generic;
using System.Linq;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.WebModels;
using VillageOfFate.WebModels.Activities;

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
			ActivityQueue = new Stack<WebActivity>(villager.ActivityQueue.Select(a => a.AsWebActivity())),
			ImageId = villager.ImageId
		};

	public static WebItem AsWebItem(this ItemDto item) => new() {
		Id = item.Id,
		Name = item.Name,
		Description = item.Description,
		Edible = item.Edible,
		HungerRestored = item.HungerRestored,
		Quantity = item.Quantity,
		ImageId = item.Image?.Id
	};

	public static WebVillagerEmotions AsWebVillagerEmotions(this EmotionDto emotions) =>
		new() {
			Happiness = emotions.Happiness,
			Sadness = emotions.Sadness,
			Fear = emotions.Fear
		};

	public static WebActivity AsWebActivity(this ActivityDto activity) => activity switch {
		IdleActivityDto => new IdleWebActivity {
			Name = activity.Name,
			Description = activity.Description,
			Interruptible = activity.Interruptible,
			Duration = activity.Duration,
			StartTime = activity.StartTime,
			EndTime = activity.EndTime
		},
		AdjustEmotionalStateActivityDto emotional => new AdjustEmotionalStateWebActivity {
			Name = activity.Name,
			Description = activity.Description,
			Interruptible = activity.Interruptible,
			Duration = activity.Duration,
			StartTime = activity.StartTime,
			EndTime = activity.EndTime,
			Adjustment = emotional.Adjustment,
			Emotion = emotional.Emotion,
			Reason = emotional.Reason
		},
		_ => throw new NotImplementedException()
	};

	public static WebSector AsWebSector(this SectorDto sector) =>
		new(sector.Position) {
			Id = sector.Id,
			Description = sector.Description,
			Items = sector.Items.Select(AsWebItem).ToList(),
			ImageId = sector.ImageId
		};
}