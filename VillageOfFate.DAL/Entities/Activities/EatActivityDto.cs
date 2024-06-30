using System;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class EatActivityDto() : ActivityDto(ActivityName.Eat) {
	public Guid TargetItemId { get; set; } = Guid.Empty;
	public required ItemDto TargetItem { get; init; }
}