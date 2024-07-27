using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class ItemService(DataContext context, ItemDefinitionService itemDefinitions, TimeService time) {
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
		var pageNumber = await context.Items.Where(i => i.ItemId == bible.Id)
									  .Select(i => i.PageNumber ?? 0)
									  .DefaultIfEmpty()
									  .MaxAsync() + 1;
		var entry = await context.Items.AddAsync(new ItemDto {
			Definition = pageDefinition,
			Content = message,
			ItemId = bible.Id,
			CreationDate = await time.GetAsync(TimeLabel.World),
			PageNumber = pageNumber
		});
		await context.SaveChangesAsync();
		return entry.Entity;
	}

	public async Task<IEnumerable<ItemDto>> GetChildItemPagesAsync(Guid containerId) {
		return await context.Items.Where(i => i.ItemId == containerId && i.Definition.Category == ItemCategory.Page)
							.ToListAsync();
	}

	public async Task<ItemDto> GetChildItemPageAsync(Guid containerId, int pageNumber) {
		var pages = context.Items.Where(i => i.ItemId == containerId && i.Definition.Category == ItemCategory.Page);
		if (pageNumber >= 0) {
			return await pages.FirstOrDefaultAsync(p => p.PageNumber == pageNumber)
				   ?? throw new IndexOutOfRangeException(
					   $"Could not locate page {pageNumber} in container {containerId}");
		}

		var max = await pages.MaxAsync(p => p.PageNumber ?? 0);
		var page = max + 1 + pageNumber;
		return await pages.FirstOrDefaultAsync(p => p.PageNumber == page)
			   ?? throw new IndexOutOfRangeException(
				   $"Could not locate page {page} ({pageNumber}) in container {containerId}");
	}
}