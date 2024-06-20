using System;
using System.Text.Json.Serialization;

namespace VillageOfFate.WebModels.Activities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Name))]
[JsonDerivedType(typeof(IdleWebActivity), (int)ActivityName.Idle)]
[JsonDerivedType(typeof(AdjustEmotionalStateWebActivity), (int)ActivityName.AdjustEmotionalState)]
public class WebActivity {
	public required ActivityName Name { get; init; }
	public DateTime StartTime { get; init; }
	public DateTime EndTime { get; init; }
	public required string Description { get; init; }
	public TimeSpan Duration { get; init; }
	public bool Interruptible { get; init; }
}