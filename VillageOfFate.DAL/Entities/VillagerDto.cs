using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities;

public class VillagerDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[MaxLength(InitialCreate.MaxNameLength)]
	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Summary { get; set; } = string.Empty;

	public Gender Gender { get; set; }
	public int Hunger { get; set; }

	public Guid SectorId { get; set; }
	[ForeignKey("SectorId")] public required SectorDto Sector { get; set; }

	public Guid ActivityId { get; set; }
	public ActivityDto Activity { get; set; } = null!;

	public List<VillagerMemoryDto> Memories { get; set; } = [];
	public List<ItemDto> Items { get; } = [];

	public static void OnModelCreating(ModelBuilder modelBuilder) { }
}