using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Items;

[Table("Items")]
public class ItemLocationDto {
	[Key] public Guid Id { get; set; } = Guid.NewGuid();

	[Column(nameof(VillagerId))] public Guid? VillagerId { get; set; }
	[ForeignKey(nameof(VillagerId))] public VillagerDto? Villager { get; set; }

	[Column(nameof(SectorId))] public Guid? SectorId { get; set; }
	[ForeignKey(nameof(SectorId))] public SectorDto? Sector { get; set; }

	public ItemDto Item { get; set; } = null!;

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemLocationDto>()
					.HasOne(i => i.Villager)
					.WithMany()
					.HasForeignKey(i => i.VillagerId);

		modelBuilder.Entity<ItemLocationDto>()
					.HasOne(i => i.Sector)
					.WithMany()
					.HasForeignKey(i => i.SectorId);
	}
}