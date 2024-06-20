using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Villagers;

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

	public Guid EmotionsId { get; set; }
	public EmotionDto Emotions { get; set; } = new();

	public Guid? ImageId { get; set; }
	public ImageDto? Image { get; set; }

	[NotMapped] public ActivityDto? CurrentActivity => Activities.MinBy(a => a.Priority);

	[NotMapped]
	public IReadOnlyCollection<ActivityDto> ActivityQueue => Activities.OrderBy(a => a.Priority).Skip(1).ToList();

	public List<ItemDto> Items { get; } = [];
	public List<ActivityDto> Activities { get; set; } = [];

	public List<EventDto> Events { get; set; } = [];

	public string GetDescription() => $"{Name} is a {Age} year old {Gender}. Summary: {Summary}";

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<VillagerDto>()
					.HasMany(v => v.Events)
					.WithMany(v => v.Witnesses)
					.UsingEntity<EventWitnessDto>();
	}
}