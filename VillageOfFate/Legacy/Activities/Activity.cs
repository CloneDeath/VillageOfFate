using System;

namespace VillageOfFate.Legacy.Activities;

public class Activity(IActivityDetails details, World world) : IActivityDetails {
	public DateTime StartTime { get; set; } = world.CurrenTime;
	public DateTime EndTime => StartTime + Duration;
	public string Description => details.Description;
	public TimeSpan Duration { get; set; } = details.Duration;
	public bool Interruptible => details.Interruptible;
	public ActivityResult OnCompletion() => details.OnCompletion();
}