using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VillageOfFate.DAL.Attributes;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Events;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<TimeDto> Time { get; set; } = null!;
	public DbSet<ItemDto> Items { get; set; } = null!;

	public DbSet<ActivityDto> Activities { get; set; } = null!;

	public DbSet<VillagerDto> Villagers { get; set; } = null!;
	public DbSet<VillagerItemDto> VillagerItems { get; set; } = null!;
	public DbSet<RelationshipDto> Relationships { get; set; } = null!;
	public DbSet<EmotionDto> Emotions { get; set; } = null!;

	public DbSet<EventDto> Events { get; set; } = null!;
	public DbSet<EventWitnessDto> EventWitnesses { get; set; } = null!;

	public DbSet<SectorDto> Sectors { get; set; } = null!;
	public DbSet<SectorItemDto> SectorItems { get; set; } = null!;

	public DbSet<GptUsageDto> GptUsage { get; set; } = null!;
	public DbSet<VillagerActionErrorDto> VillagerActionErrors { get; set; } = null!;
	public DbSet<ImageDto> Images { get; set; }

	public DbSet<UserDto> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
			foreach (var property in entityType.GetProperties()) {
				if (property.PropertyInfo == null) continue;
				if (Attribute.IsDefined(property.PropertyInfo, typeof(UtcDateTimeAttribute))) {
					modelBuilder.Entity(entityType.Name).Property(property.Name)
								.HasConversion(new ValueConverter<DateTime, DateTime>(
									v => v.ToUniversalTime(),
									v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
				} else if (Attribute.IsDefined(property.PropertyInfo, typeof(TimeSpanTicksAttribute))) {
					modelBuilder.Entity(entityType.Name).Property(property.Name)
								.HasConversion(new ValueConverter<TimeSpan, long>(
									v => v.Ticks,
									v => new TimeSpan(v)));
				}
			}
		}

		VillagerDto.OnModelCreating(modelBuilder);
		EventDto.OnModelCreating(modelBuilder);
		TimeDto.OnModelCreating(modelBuilder);
		ItemDto.OnModelCreating(modelBuilder);
		ActivityDto.OnModelCreating(modelBuilder);
		InteractActivityTargetDto.OnModelCreating(modelBuilder);
		AdjustEmotionalStateActivityDto.OnModelCreating(modelBuilder);
	}
}