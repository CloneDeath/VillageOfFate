using System;

namespace VillageOfFate.WebModels;

public class WebVillager {
	public Guid Id { get; set; } = Guid.Empty;
	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;
	public string Summary { get; set; } = string.Empty;
	public Gender Gender { get; set; } = Gender.Male;
	public WebVillagerEmotions Emotions { get; set; } = new();
	public Position SectorLocation { get; set; }
}