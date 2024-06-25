using System;

namespace VillageOfFate.WebModels.Activities;

public class InteractWebActivity : WebActivity {
	public required string Action { get; init; }
	public required Guid[] TargetIds { get; init; }
}