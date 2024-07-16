using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities.Items;

public class ItemDefinitionDto {
	[Key] public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxNameLength)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public Guid ImageId { get; set; }
	public required ImageDto Image { get; set; }

	public List<ItemDto> Items { get; set; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemDefinitionDto>()
					.HasMany(i => i.Items)
					.WithOne(i => i.ItemDefinition)
					.HasForeignKey(i => i.ItemDefinitionId);

		modelBuilder.Entity<ItemDefinitionDto>()
					.HasOne(i => i.Image)
					.WithOne(i => i.ItemDefinition)
					.HasForeignKey<ItemDefinitionDto>(i => i.ImageId);
	}
}