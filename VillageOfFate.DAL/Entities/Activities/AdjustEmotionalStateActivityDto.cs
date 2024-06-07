using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class AdjustEmotionalStateActivityDto : ActivityDto {
	public AdjustEmotionalStateActivityDto(TimeSpan duration) : this() {
		Duration = duration;
	}

	public AdjustEmotionalStateActivityDto() {
		Name = ActivityName.AdjustEmotionalState;
		Description = "{villager} adjusts their emotional state";
		Interruptible = true;
	}

	public VillagerEmotion Emotion { get; set; }
	public int Adjustment { get; set; }

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Reason { get; set; } = string.Empty;

	public new static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<AdjustEmotionalStateActivityDto>()
					.Property(e => e.Emotion)
					.HasConversion<string>();
	}
}