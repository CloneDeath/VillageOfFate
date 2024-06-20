using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Attributes;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Events;

public class EventDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	[UtcDateTime] public DateTime Time { get; set; }

	public Guid SectorId { get; set; }
	public required SectorDto Sector { get; set; }

	public Guid ActorId { get; set; }
	public required VillagerDto? Actor { get; set; }
}