using System;
using Microsoft.EntityFrameworkCore;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<Time> Time { get; set; } = null!;
	public DbSet<ItemDto> Items { get; set; } = null!;

	public DbSet<VillagerDto> Villagers { get; set; } = null!;
	public DbSet<VillagerItemDto> VillagerItems { get; set; } = null!;

	public DbSet<SectorDto> Sectors { get; set; } = null!;
	public DbSet<SectorItemDto> SectorItems { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Time>().Property(e => e.Now)
					.HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

		modelBuilder.Entity<ItemDto>().Property(e => e.Name).HasMaxLength(InitialCreate.MaxNameLength);
		modelBuilder.Entity<ItemDto>().Property(e => e.Description).HasMaxLength(InitialCreate.MaxDescriptionLength);

		modelBuilder.Entity<VillagerDto>().Property(e => e.Name).HasMaxLength(InitialCreate.MaxNameLength);
		modelBuilder.Entity<VillagerDto>().Property(e => e.Summary).HasMaxLength(InitialCreate.MaxDescriptionLength);

		modelBuilder.Entity<SectorDto>().Property(e => e.Description).HasMaxLength(InitialCreate.MaxDescriptionLength);
	}
}