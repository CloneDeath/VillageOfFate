using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VillageOfFate.DAL.Attributes;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<TimeDto> Time { get; set; } = null!;
	public DbSet<ItemDto> Items { get; set; } = null!;

	public DbSet<ActivityDto> Activities { get; set; } = null!;

	public DbSet<VillagerDto> Villagers { get; set; } = null!;
	public DbSet<VillagerItemDto> VillagerItems { get; set; } = null!;
	public DbSet<RelationshipDto> Relationships { get; set; } = null!;
	public DbSet<VillagerMemoryDto> VillagerMemories { get; set; } = null!;
	public DbSet<EmotionDto> Emotions { get; set; } = null!;

	public DbSet<SectorDto> Sectors { get; set; } = null!;
	public DbSet<SectorItemDto> SectorItems { get; set; } = null!;

	public DbSet<GptUsageDto> GptUsage { get; set; } = null!;
	public DbSet<VillagerActionErrorDto> VillagerActionErrors { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
			foreach (var property in entityType.GetProperties()) {
				if (property.PropertyInfo != null &&
					Attribute.IsDefined(property.PropertyInfo, typeof(UtcDateTimeAttribute))) {
					modelBuilder.Entity(entityType.Name).Property(property.Name)
								.HasConversion(new ValueConverter<DateTime, DateTime>(
									v => v.ToUniversalTime(),
									v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
				}
			}
		}

		modelBuilder.Entity<TimeDto>().Property(e => e.Label)
					.HasConversion<string>();

		modelBuilder
			.Entity<ActivityDto>()
			.Property(d => d.Name)
			.HasConversion<string>();

		// TPH pattern
		// https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
		modelBuilder.Entity<ActivityDto>()
					.HasDiscriminator(p => p.Name)
					.HasValue<IdleActivityDto>(ActivityName.Idle);

		ItemDto.OnModelCreating(modelBuilder);
		ActivityDto.OnModelCreating(modelBuilder);
	}
}