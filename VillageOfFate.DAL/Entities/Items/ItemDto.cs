using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Items;

[Table("Items")]
public class ItemDto {
	[Key] public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxNameLength)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public int Quantity { get; set; } = 1;
	public bool Edible { get; set; }
	public int HungerRestored { get; set; }

	public Guid ImageId { get; set; }
	public required ImageDto Image { get; set; }

	[Column(nameof(VillagerId))] public Guid? VillagerId { get; set; }
	[ForeignKey(nameof(VillagerId))] public VillagerDto? Villager { get; set; }

	[Column(nameof(SectorId))] public Guid? SectorId { get; set; }
	[ForeignKey(nameof(SectorId))] public SectorDto? Sector { get; set; }

	public List<EventDto> ActorEvents { get; set; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemDto>()
					.HasOne(i => i.Villager)
					.WithMany(v => v.Items)
					.HasForeignKey(i => i.VillagerId);

		modelBuilder.Entity<ItemDto>()
					.HasOne(i => i.Sector)
					.WithMany(s => s.Items)
					.HasForeignKey(i => i.SectorId);

		modelBuilder.Entity<ItemDto>()
					.HasMany(i => i.ActorEvents)
					.WithOne(e => e.ItemActor)
					.HasForeignKey(e => e.ItemActorId);
	}

	public string GetSummary() {
		var edibleString = Edible ? $"Edible (-{HungerRestored} hunger)" : "";
		return $"{Name} (Id: {Id}): {Description} Quantity: {Quantity} {edibleString}";
	}
}