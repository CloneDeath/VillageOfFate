using VillageOfFate.WebModels;

namespace VillageOfFate.Server;

public static class WebModelConversions {
	public static WebVillager AsWebVillager(this Villager villager) {
		return new WebVillager {
			Id = villager.Id,
			Name = villager.Name,
			Gender = villager.Gender,
			Summary = villager.Summary,
			Age = villager.Age,
			Emotions = AsWebVillagerEmotions(villager.Emotions),
			SectorLocation = villager.SectorLocation
		};
	}

	public static WebVillagerEmotions AsWebVillagerEmotions(this VillagerEmotions emotions) {
		return new WebVillagerEmotions {
			Happiness = emotions.Happiness,
			Sadness = emotions.Sadness,
			Fear = emotions.Fear
		};
	}
}