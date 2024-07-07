using System;

namespace VillageOfFate.WebModels;

public class WebItemLocation {
	public required WebVillagerLocation? Villager { get; init; }
	public required WebSectorLocation? Sector { get; init; }
}

public class WebVillagerLocation {
	public required Guid Id { get; init; }
	public required WebSectorLocation Sector { get; init; }
}

public class WebSectorLocation {
	public required Guid Id { get; init; }
	public required Position Position { get; init; }
}