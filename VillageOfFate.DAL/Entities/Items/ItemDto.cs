using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Events;

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

	public List<EventDto> ActorEvents { get; set; } = [];

	[ForeignKey(nameof(Id))] public ItemLocationDto Location { get; set; } = null!;

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ItemDto>()
					.HasOne(i => i.Location)
					.WithOne(i => i.Item)
					.HasForeignKey<ItemLocationDto>(i => i.Id);
	}

	public string GetSummary() {
		var edibleString = Edible ? $"Edible (-{HungerRestored} hunger)" : "";
		return $"{Name} (Id: {Id}): {Description} Quantity: {Quantity} {edibleString}";
	}
}