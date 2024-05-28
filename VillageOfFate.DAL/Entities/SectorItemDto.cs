using System;

namespace VillageOfFate.DAL.Entities;

public class SectorItemDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid SectorId { get; set; } = Guid.Empty;
	public Guid ItemId { get; set; } = Guid.Empty;
}