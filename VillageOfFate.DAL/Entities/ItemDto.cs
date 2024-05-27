using System;

namespace VillageOfFate.DAL.Entities;

public class ItemDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int Quantity { get; set; } = 1;
	public bool Edible { get; set; }
	public int HungerRestored { get; set; }
}