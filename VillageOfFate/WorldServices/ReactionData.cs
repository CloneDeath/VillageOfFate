using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.WorldServices;

public class ReactionData {
	public required VillagerDto? Actor { get; init; }
	public required ItemDto? Item { get; init; }
	public required string ActiveActionName { get; init; }
}