using System;

namespace VillageOfFate.DAL.Entities.Villagers;

public class VillagerItemDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	public Guid VillagerId { get; set; } = Guid.Empty;
	public required VillagerDto Villager { get; set; }

	public Guid ItemId { get; set; } = Guid.Empty;
	public required ItemDto Item { get; set; }
}