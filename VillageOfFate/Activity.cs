using System;

namespace VillageOfFate;

public class Activity {
	public required string Description { get; init; }
	public required DateTime StartTime { get; init; }
	public required TimeSpan Duration { get; init; }
	public required Action OnCompletion { get; init; }
}