using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Attributes;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities;

public abstract class ActivityDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	public ActivityName Name { get; protected init; }

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public int Priority { get; set; } = int.MaxValue;

	public long DurationTicks { get; set; }

	[NotMapped]
	public TimeSpan Duration {
		get => new(DurationTicks);
		set => DurationTicks = value.Ticks;
	}

	public bool Interruptible { get; set; }

	[UtcDateTime] public DateTime StartTime { get; set; }
	public DateTime EndTime => StartTime + Duration;

	public Guid VillagerId { get; set; }
	[ForeignKey(nameof(VillagerId))] public VillagerDto Villager { get; set; } = null!;

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		// TPH pattern - https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
		modelBuilder.Entity<ActivityDto>()
					.HasDiscriminator(p => p.Name)
					.HasValue<IdleActivityDto>(ActivityName.Idle)
					.HasValue<AdjustEmotionalStateActivityDto>(ActivityName.AdjustEmotionalState)
					.HasValue<EatActivityDto>(ActivityName.Eat)
					.HasValue<InteractActivityDto>(ActivityName.Interact)
					.HasValue<LookoutActivityDto>(ActivityName.Lookout)
					.HasValue<SleepActivityDto>(ActivityName.Sleep)
					.HasValue<SpeakActivityDto>(ActivityName.Speak);

		modelBuilder.Entity<ActivityDto>()
					.Property(d => d.Name)
					.HasConversion<string>();

		modelBuilder.Entity<ActivityDto>()
					.HasOne(v => v.Villager)
					.WithMany(v => v.Activities);
	}
}