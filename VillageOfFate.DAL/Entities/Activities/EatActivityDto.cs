using System;

namespace VillageOfFate.DAL.Entities.Activities;

public class EatActivityDto : ActivityDto {
	public Guid TargetItemId { get; set; } = Guid.Empty;
	public required ItemDto TargetItem { get; init; }
}