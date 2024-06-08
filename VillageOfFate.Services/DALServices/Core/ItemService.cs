using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices.Core;

public class ItemService(DataContext context, ImageService image) {
	public async Task<ItemDto> EnsureExistsAsync(ItemDto item) {
		var result = await context.Items.Include(i => i.Image).FirstOrDefaultAsync(i => i.Id == item.Id)
					 ?? (await context.Items.AddAsync(item)).Entity;

		if (result.ImageId == null) {
			result.Image = await image.GenerateImageAsync($"{item.Name}, {item.Description}");
		}

		await context.SaveChangesAsync();
		return result;
	}
}