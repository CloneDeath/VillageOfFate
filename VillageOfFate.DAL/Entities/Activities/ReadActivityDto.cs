using System;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class ReadActivityDto() : ActivityDto(ActivityName.Read) {
	public Guid TargetItemId { get; set; }
	public ItemDto TargetItem { get; set; } = null!;
	public ReadingMode ReadingMode { get; set; }

	public new static void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.Entity<ReadActivityDto>()
					.HasOne(v => v.TargetItem)
					.WithMany()
					.HasForeignKey(a => a.TargetItemId);

		modelBuilder.Entity<ReadActivityDto>()
					.Property(a => a.ReadingMode)
					.HasConversion<string>();
	}
}

public enum ReadingMode {
	SilentReading,
	ReadAloud
}