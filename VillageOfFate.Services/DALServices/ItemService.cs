using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class ItemService(DataContext context, ItemDefinitionService itemDefinitions) {
	public async Task<ItemDto> EnsureExistsAsync(ItemDto item) {
		var result = await context.Items.FirstOrDefaultAsync(i => i.Id == item.Id)
					 ?? (await context.Items.AddAsync(item)).Entity;

		await context.SaveChangesAsync();
		return result;
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
			throw new Exception($"Villager {villager.Name} does not have access to {item.Definition.Name}!");
		}

		await context.SaveChangesAsync();
	}

	public async Task<ItemDto> CreateBiblePageAsync(string message, ItemDto bible) {
		var pageDefinition = await itemDefinitions.GetOrCreateBiblePageAsync();
		var entry = await context.Items.AddAsync(new ItemDto {
			Definition = pageDefinition,
			Content = message,
			ItemId = bible.Id
		});
		await context.SaveChangesAsync();
		return entry.Entity;
	}

	public async Task<IEnumerable<ItemDto>> GetChildItemPagesAsync(Guid itemId) {
		return await context.Items.Where(i => i.ItemId == itemId && i.Definition.Category == ItemCategory.Page)
							.ToListAsync();
	}
}