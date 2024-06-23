using System;

namespace VillageOfFate.DAL.Entities.Activities;

public class EatActivityDto : ActivityDto {
	public Guid TargetItemId { get; set; } = Guid.Empty;
	public ItemDto TargetItem { get; set; } = null!;
}