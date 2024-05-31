using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class VillagerItemService(DataContext context) {
	public async Task AddAsync(VillagerDto villager, ItemDto item) {
		// var itemDto = await items.EnsureExistsAsync(item);
		await context.VillagerItems.AddAsync(new VillagerItemDto {
			Villager = villager,
			Item = item
		});
		await context.SaveChangesAsync();
	}
}