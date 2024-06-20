using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Attributes;

namespace VillageOfFate.DAL.Entities.Villagers;

public class VillagerActionErrorDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[UtcDateTime] public DateTime WorldTime { get; set; }
	[UtcDateTime] public DateTime EarthTime { get; set; }

	public Guid VillagerId { get; set; }
	public VillagerDto Villager { get; set; } = null!;

	[MaxLength(InitialCreate.MaxNameLength)]
	public string ActionName { get; set; } = string.Empty;
	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Arguments { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Error { get; set; } = string.Empty;
}