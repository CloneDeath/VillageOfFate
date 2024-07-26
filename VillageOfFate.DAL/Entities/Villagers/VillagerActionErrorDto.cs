using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Attributes;
using VillageOfFate.DAL.Entities.Activities;

namespace VillageOfFate.DAL.Entities.Villagers;

public class VillagerActionErrorDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	[UtcDateTime] public DateTime WorldTime { get; set; }
	[UtcDateTime] public DateTime EarthTime { get; set; }

	public Guid VillagerId { get; set; }
	public VillagerDto Villager { get; set; } = null!;

	public Guid? ActivityId { get; set; }
	public ActivityDto? Activity { get; set; }

	[MaxLength(InitialCreate.MaxNameLength)]
	public string ActionName { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Arguments { get; set; } = string.Empty;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Error { get; set; } = string.Empty;

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<VillagerActionErrorDto>()
					.HasOne(e => e.Villager)
					.WithMany()
					.HasForeignKey(e => e.VillagerId);

		modelBuilder.Entity<VillagerActionErrorDto>()
					.HasOne(e => e.Activity)
					.WithMany()
					.HasForeignKey(e => e.ActivityId);
	}
}