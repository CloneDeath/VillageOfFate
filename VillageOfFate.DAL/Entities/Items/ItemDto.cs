using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Items;

[Table("Items")]
public class ItemDto {
	[Key] public Guid Id { get; set; } = Guid.NewGuid();

	public int Quantity { get; set; } = 1;

	[Column(nameof(VillagerId))] public Guid? VillagerId { get; set; }
	[ForeignKey(nameof(VillagerId))] public VillagerDto? Villager { get; set; }

	[Column(nameof(SectorId))] public Guid? SectorId { get; set; }
	[ForeignKey(nameof(SectorId))] public SectorDto? Sector { get; set; }

	public Guid ItemDefinitionId { get; set; }
	[ForeignKey(nameof(ItemDefinitionId))] public ItemDefinitionDto Definition { get; set; } = null!;

	public List<EventDto> ActorEvents { get; set; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemDto>()
					.Navigation(i => i.Definition)
					.AutoInclude();

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
		var edibleString = Definition.Edible ? $"Edible (-{Definition.HungerRestored} hunger)" : "";
		return $"{Definition.Name} (Id: {Id}): {Definition.Description} Quantity: {Quantity} {edibleString}";
	}
}