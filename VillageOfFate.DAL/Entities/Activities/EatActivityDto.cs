using System;
using System.ComponentModel.DataAnnotations.Schema;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class EatActivityDto() : ActivityDto(ActivityName.Eat) {
	[Column(nameof(TargetItemId))] public Guid TargetItemId { get; set; } = Guid.Empty;
	public required ItemDto TargetItem { get; init; }
}