using System;

namespace VillageOfFate.WebModels;

public class WebVillager {
	public Guid Id { get; set; } = Guid.Empty;
	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;
	public string Summary { get; set; } = string.Empty;
	public WebGender Gender { get; set; } = WebGender.Male;
}