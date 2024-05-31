using System;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.DAL;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {
	public DbSet<Time> Time { get; set; } = null!;
	public DbSet<ItemDto> Items { get; set; } = null!;

	public DbSet<VillagerDto> Villagers { get; set; } = null!;
	public DbSet<VillagerItemDto> VillagerItems { get; set; } = null!;
	public DbSet<RelationshipDto> Relationships { get; set; } = null!;

	public DbSet<SectorDto> Sectors { get; set; } = null!;
	public DbSet<SectorItemDto> SectorItems { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Time>().Property(e => e.Now)
					.HasConversion(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

		// modelBuilder.Entity<Blog>()
		// 			.HasMany(e => e.Posts)
		// 			.WithOne(e => e.Blog)
		// 			.HasForeignKey(e => e.BlogId)
		// 			.HasPrincipalKey(e => e.Id);

		// For Activities
		// https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
		// modelBuilder.Entity<Blog>()
		// 			.HasDiscriminator<string>("blog_type")
		// 			.HasValue<Blog>("blog_base")
		// 			.HasValue<RssBlog>("blog_rss");
		// modelBuilder.Entity<Blog>()
		// .HasDiscriminator(b => b.BlogType);
	}
}