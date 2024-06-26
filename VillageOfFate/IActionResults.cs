using System.Collections.Generic;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate;

public interface IActionResults {
	public List<VillagerDto> TriggerReactions { get; }
}

public class ActionResults : IActionResults {
	public List<VillagerDto> TriggerReactions { get; init; } = [];
}