using System;

namespace VillageOfFate.Activities;

public class Activity(IActivityDetails details, World world) : IActivityDetails {
	public DateTime StartTime { get; } = world.CurrenTime;
	public string Description => details.Description;
	public TimeSpan Duration => details.Duration;
	public void OnCompletion() => details.OnCompletion();
}