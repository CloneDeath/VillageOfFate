using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities;

public class ItemDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxNameLength)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public int Quantity { get; set; } = 1;
	public bool Edible { get; set; }
	public int HungerRestored { get; set; }
}