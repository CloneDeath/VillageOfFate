using System.Collections.Generic;

namespace VillageOfFate.Legacy.Activities;

public class ActivityResult {
	public required IEnumerable<Villager> TriggerReactions { get; init; }
}