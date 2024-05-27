using System;

namespace VillageOfFate.DAL.Entities;

public class VillagerDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;
	public string Summary { get; set; } = string.Empty;
	public Gender Gender { get; set; }
	public int Hunger { get; set; }
}