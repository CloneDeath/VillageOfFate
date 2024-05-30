using System;

namespace VillageOfFate.DAL.Entities;

public class RelationshipDto {
	public Guid Id { get; set; }
	public Guid VillagerId { get; set; }
	public Guid RelationId { get; set; }

	public string Summary { get; set; } = string.Empty;
}