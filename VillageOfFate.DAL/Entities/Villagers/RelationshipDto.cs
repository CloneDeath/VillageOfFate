using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities.Villagers;

public class RelationshipDto {
	public Guid Id { get; set; }

	public Guid VillagerId { get; set; }
	public required VillagerDto Villager { get; set; }

	public Guid RelationId { get; set; }
	public required VillagerDto Relation { get; set; }

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Summary { get; set; } = string.Empty;
}