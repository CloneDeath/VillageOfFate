using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities;

public class ItemDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxNameLength)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public int Quantity { get; set; } = 1;
	public bool Edible { get; set; }
	public int HungerRestored { get; set; }

	public Guid ImageId { get; set; }
	public ImageDto Image { get; set; } = null!;

	public List<VillagerDto> Villagers { get; } = [];
	public List<SectorDto> Sectors { get; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemDto>()
					.HasMany(v => v.Villagers)
					.WithMany(v => v.Items)
					.UsingEntity<VillagerItemDto>();

		modelBuilder.Entity<ItemDto>()
					.HasMany(v => v.Sectors)
					.WithMany(v => v.Items)
					.UsingEntity<SectorItemDto>();
	}

	public string GetSummary() {
		var edibleString = Edible ? $"Edible (-{HungerRestored} hunger)" : "";
		return $"{Name} (Id: {Id}): {Description} Quantity: {Quantity} {edibleString}";
	}
}