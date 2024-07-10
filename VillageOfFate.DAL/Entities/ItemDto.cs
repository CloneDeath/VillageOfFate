using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Villagers;

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

	public Guid? VillagerId { get; set; }
	[ForeignKey(nameof(VillagerId))] public VillagerDto? Villager { get; set; }

	public Guid? SectorId { get; set; }
	[ForeignKey(nameof(SectorId))] public SectorDto? Sector { get; set; }

	public IEnumerable<EventDto> ActorEvents { get; set; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// modelBuilder.Entity<ItemDto>()
		// 			.Navigation(i => i.Image).AutoInclude();
	}

	public string GetSummary() {
		var edibleString = Edible ? $"Edible (-{HungerRestored} hunger)" : "";
		return $"{Name} (Id: {Id}): {Description} Quantity: {Quantity} {edibleString}";
	}
}