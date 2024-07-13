using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Attributes;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Events;

public class EventDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	[UtcDateTime] public DateTime Time { get; set; }
	public int Order { get; set; }

	public Guid SectorId { get; set; }
	public SectorDto Sector { get; set; } = null!;

	public Guid? VillagerActorId { get; set; }
	public VillagerDto? VillagerActor { get; set; }

	public Guid? ItemActorId { get; set; }
	public ItemDto? ItemActor { get; set; }

	public List<VillagerDto> Witnesses { get; set; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<EventDto>()
					.HasOne(e => e.VillagerActor)
					.WithMany(v => v.ActorEvents)
					.HasForeignKey(e => e.VillagerActorId);

		modelBuilder.Entity<EventDto>()
					.HasOne(e => e.ItemActor)
					.WithMany(v => v.ActorEvents)
					.HasForeignKey(e => e.ItemActorId);
	}
}