using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities;

public abstract class ActivityDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	public ActivityName Name { get; protected set; }

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public long DurationTicks { get; set; }

	[NotMapped]
	public TimeSpan Duration {
		get => new(DurationTicks);
		set => DurationTicks = value.Ticks;
	}

	public bool Interruptible { get; set; }

	public required DateTime StartTime { get; set; }
	public DateTime EndTime => StartTime + Duration;

	public Guid VillagerId { get; set; }
	[ForeignKey(nameof(VillagerId))] public VillagerDto Villager { get; set; } = null!;

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ActivityDto>()
					.HasOne(v => v.Villager)
					.WithMany(v => v.Activities);
	}
}

public enum ActivityName {
	Idle
}