using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices.Core;

public class ItemService(DataContext context) {
	public async Task<ItemDto> EnsureExistsAsync(ItemDto item) {
		var result = await context.Items.Include(i => i.Image).FirstOrDefaultAsync(i => i.Id == item.Id)
					 ?? (await context.Items.AddAsync(item)).Entity;

		await context.SaveChangesAsync();
		return result;
	}

	public IEnumerable<ItemDto> GetItemsWithoutImages() {
		return context.Items.Where(i => i.ImageId == null);
	}
}