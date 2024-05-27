using System;

namespace VillageOfFate.DAL.Entities;

public class VillagerItemDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid VillagerId { get; set; } = Guid.Empty;
	public Guid ItemID { get; set; } = Guid.Empty;
}