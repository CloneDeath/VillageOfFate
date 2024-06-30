using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Runners;

public class ReactionData {
	public required VillagerDto Actor { get; init; }
	public required ActivityDto Action { get; init; }
}