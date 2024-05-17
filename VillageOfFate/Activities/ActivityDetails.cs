using System;

namespace VillageOfFate.Activities;

public interface IActivityDetails {
	public string Description { get; }
	public TimeSpan Duration { get; }
	public void OnCompletion();
}

public class ActivityDetails : IActivityDetails {
	public Action? OnCompletion { get; init; }
	public required string Description { get; init; }
	public required TimeSpan Duration { get; init; }

	void IActivityDetails.OnCompletion() => OnCompletion?.Invoke();
}

public class IdleActivity : IActivityDetails {
	public string Description => "Idle";
	public TimeSpan Duration => TimeSpan.Zero;
	public void OnCompletion() { }
}