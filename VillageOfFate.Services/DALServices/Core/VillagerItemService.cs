using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerItemService(DataContext context) {
	public async Task<ItemDto> AddAsync(VillagerDto villager, ItemDto item) {
		var result = await context.VillagerItems.AddAsync(new VillagerItemDto {
			Villager = villager,
			Item = item
		});
		await context.SaveChangesAsync();
		return result.Entity.Item;
	}
}