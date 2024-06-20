using System;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Events;

public class EventWitnessDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	public Guid EventId { get; set; }
	public required EventDto Event { get; set; }

	public Guid VillagerId { get; set; }
	public required VillagerDto Villager { get; set; }
}