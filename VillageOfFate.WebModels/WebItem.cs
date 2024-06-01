using System;

namespace VillageOfFate.WebModels;

public class WebItem {
	public Guid Id { get; init; } = Guid.NewGuid();

	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int Quantity { get; set; } = 1;
	public bool Edible { get; set; }
	public int HungerRestored { get; set; }

	public string GetSummary() {
		var edibleString = Edible ? $"Edible (-{HungerRestored} hunger)" : "";
		return $"{Name} (Id: {Id}): {Description} Quantity: {Quantity} {edibleString}";
	}
}