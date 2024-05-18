using System.Collections.Generic;

namespace VillageOfFate.Activities;

public class ActivityResult {
	public required IEnumerable<Villager> TriggerReactions { get; init; }
}