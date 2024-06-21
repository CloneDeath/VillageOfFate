using System;

namespace VillageOfFate.WebModels;

public class WebEvent {
	public required Guid Id { get; init; }
	public required DateTime Time { get; init; }
	public required Position Sector { get; init; }
	public required string Description { get; init; }
	public required Guid? ActorId { get; init; }
	public required Guid[] WitnessIds { get; init; }
}