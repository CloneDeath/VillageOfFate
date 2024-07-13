using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerItemService(DataContext context) {
	public async Task<ItemDto> AddAsync(VillagerDto villager, ItemDto item) {
		item.Location = new ItemLocationDto {
			VillagerId = villager.Id,
			SectorId = null
		};
		var result = await context.Items.AddAsync(item);
		await context.SaveChangesAsync();
		return result.Entity;
	}
}