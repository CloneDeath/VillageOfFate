using System;

namespace VillageOfFate;

public class Item {
	public Guid Id { get; } = Guid.NewGuid();

	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int Quantity { get; set; } = 1;
	public bool Edible { get; set; } = false;
	public int HungerRestored { get; set; } = 0;
}