using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class AdjustEmotionalStateActivityDto : ActivityDto {
	public AdjustEmotionalStateActivityDto() {
		Name = ActivityName.AdjustEmotionalState;
	}

	public required VillagerEmotion Emotion { get; init; }
	public required int Adjustment { get; init; }

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public required string Reason { get; init; }

	public new static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<AdjustEmotionalStateActivityDto>()
					.Property(e => e.Emotion)
					.HasConversion<string>();
	}
}