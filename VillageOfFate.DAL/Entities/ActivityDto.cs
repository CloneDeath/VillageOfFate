using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities;

public class ActivityDto {
	public Guid Id { get; set; } = Guid.NewGuid();

	public ActivityName Name { get; set; } = ActivityName.Idle;
	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public long DurationTicks { get; set; }

	public TimeSpan Duration {
		get => new(DurationTicks);
		set => DurationTicks = value.Ticks;
	}

	public bool Interruptible { get; set; }

	public VillagerDto Villager { get; set; } = null!;
	public required DateTime StartTime { get; set; }
	public DateTime EndTime => StartTime + Duration;
}

public enum ActivityName {
	Idle
}