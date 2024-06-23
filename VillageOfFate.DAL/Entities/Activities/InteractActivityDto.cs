using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL.Entities.Activities;

public class InteractActivityDto : ActivityDto {
	public VillagerDto[] Targets { get; set; } = [];

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Action { get; set; } = string.Empty;
}

public class InteractActivityTargetDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	public Guid ActivityId { get; set; } = Guid.Empty;
	public InteractActivityDto Activity { get; set; } = null!;

	public Guid VillagerId { get; set; } = Guid.Empty;
	public VillagerDto Villager { get; set; } = null!;

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<InteractActivityDto>()
					.HasMany(v => v.Targets)
					.WithMany()
					.UsingEntity<InteractActivityTargetDto>();
	}
}