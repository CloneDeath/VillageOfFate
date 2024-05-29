using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class ItemService(DataContext context) {
	public async Task<ItemDto> EnsureExistsAsync(ItemDto item) {
		var result = await context.Items.FirstOrDefaultAsync(i => i.Id == item.Id)
					 ?? (await context.Items.AddAsync(item)).Entity;
		await context.SaveChangesAsync();
		return result;
	}
}