using System;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<Time> Time { get; set; } = null!;

	public DbSet<VillagerDto> Villagers { get; set; } = null!;
	public DbSet<ItemDto> Items { get; set; } = null!;
	public DbSet<VillagerItemDto> VillagerItems { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		modelBuilder
			.Entity<Time>()
			.Property(e => e.Now)
			.HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
	}
}