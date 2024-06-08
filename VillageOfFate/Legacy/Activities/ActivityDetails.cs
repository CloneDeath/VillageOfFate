using System;

namespace VillageOfFate.Legacy.Activities;

public interface IActivityDetails {
	public string Description { get; }
	public TimeSpan Duration { get; }
	public bool Interruptible { get; }
	public ActivityResult OnCompletion();
}

public class ActivityDetails : IActivityDetails {
	public Func<ActivityResult>? OnCompletion { get; init; }
	public required string Description { get; init; }
	public required TimeSpan Duration { get; init; }
	public required bool Interruptible { get; init; }

	ActivityResult IActivityDetails.OnCompletion() =>
		OnCompletion?.Invoke() ?? new ActivityResult { TriggerReactions = [] };
}