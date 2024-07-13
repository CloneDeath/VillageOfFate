using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Items;
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
	[ForeignKey(nameof(SectorId))] public required SectorDto Sector { get; set; }

	public Guid EmotionsId { get; set; }
	public EmotionDto Emotions { get; set; } = new();

	public Guid ImageId { get; set; }
	public required ImageDto Image { get; set; }

	[NotMapped]
	public ActivityDto? CurrentActivity => Activities.FirstOrDefault(a => a.Status == ActivityStatus.InProgress);

	[NotMapped]
	public IReadOnlyCollection<ActivityDto> ActivityQueue => Activities
															 .Where(a => a.Status is ActivityStatus.Pending
																			 or ActivityStatus.OnHold)
															 .OrderBy(a => a.Priority)
															 .ToList();

	public List<ItemDto> Items { get; set; } = [];
	public List<ActivityDto> Activities { get; set; } = [];

	public List<EventDto> ActorEvents { get; set; } = [];

	public List<EventDto> WitnessedEvents { get; set; } = [];

	public string GetDescription() => $"{Name} is a {Age} year old {Gender}. Summary: {Summary}";

	public static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<VillagerDto>()
					.HasMany(v => v.WitnessedEvents)
					.WithMany(v => v.Witnesses)
					.UsingEntity<EventWitnessDto>();

		modelBuilder.Entity<VillagerDto>()
					.HasMany(v => v.Items)
					.WithOne()
					.HasForeignKey(nameof(ItemLocationDto.VillagerId));
	}
}