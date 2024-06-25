using System;

namespace VillageOfFate.WebModels.Activities;

public class EatWebActivity : WebActivity {
	public required Guid TargetItemId { get; init; }
}