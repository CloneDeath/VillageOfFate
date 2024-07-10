using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices.Core;

public class ItemService(DataContext context) {
	public async Task<ItemDto> EnsureExistsAsync(ItemDto item) {
		var result = await context.Items.Include(i => i.Image).FirstOrDefaultAsync(i => i.Id == item.Id)
					 ?? (await context.Items.AddAsync(item)).Entity;

		await context.SaveChangesAsync();
		return result;
	}

	public async Task<IEnumerable<ItemDto>> GetItemsWithoutImagesAsync() {
		return await context.Items.Where(i => i.Image.Base64Image == null).ToListAsync();
	}

	public async Task<ItemDto> GetAsync(Guid itemId) => await context.Items.FirstAsync(i => i.Id == itemId);

	public async Task<ItemDto> GetWithLocationAsync(Guid itemId) => await context.Items
																		.Include(i => i.Villager)
																		.ThenInclude(v => v!.Sector)
																		.Include(i => i.Sector)
																		.FirstAsync(i => i.Id == itemId);

	public async Task ConsumeSingle(VillagerDto villager, ItemDto item) {
		villager = context.Villagers.Entry(villager).Entity;
		item = context.Items.Entry(item).Entity;
		var sector = context.Sectors.Entry(villager.Sector).Entity;

		if (villager.Items.Any(i => i.Id == item.Id)) {
			if (item.Quantity > 1) {
				item.Quantity -= 1;
			} else {
				villager.Items.Remove(item);
			}
		} else if (sector.Items.Any(i => i.Id == item.Id)) {
			if (item.Quantity > 1) {
				item.Quantity -= 1;
			} else {
				sector.Items.Remove(item);
			}
		} else {
			throw new Exception($"Villager {villager.Name} does not have access to {item.Name}!");
		}

		await context.SaveChangesAsync();
	}
}