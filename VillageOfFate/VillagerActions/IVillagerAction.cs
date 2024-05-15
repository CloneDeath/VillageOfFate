using System.Collections.Generic;

namespace VillageOfFate.VillagerActions;

public interface IVillagerAction {
	public string Name { get; }
	public string Description { get; }
	public object? Parameters { get; }

	public void Execute(string arguments, VillagerActionState state);
}

public record VillagerActionState {
	public required Villager Actor { get; init; }
	public required IEnumerable<Villager> Others { get; init; }
}