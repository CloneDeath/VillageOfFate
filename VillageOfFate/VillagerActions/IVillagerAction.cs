using System.Collections.Generic;
using VillageOfFate.Activities;

namespace VillageOfFate.VillagerActions;

public interface IVillagerAction {
	public string Name { get; }
	public string Description { get; }
	public object? Parameters { get; }

	public IActivityDetails Execute(string arguments, VillagerActionState state);
}

public record VillagerActionState {
	public required World World { get; init; }
	public required Villager Actor { get; init; }
	public required IEnumerable<Villager> Others { get; init; }
}