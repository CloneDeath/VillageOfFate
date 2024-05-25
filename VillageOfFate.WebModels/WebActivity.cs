using System;

namespace VillageOfFate.WebModels;

public class WebActivity {
	public DateTime StartTime { get; init; }
	public DateTime EndTime { get; init; }
	public required string Description { get; init; }
	public TimeSpan Duration { get; init; }
	public bool Interruptible { get; init; }
}