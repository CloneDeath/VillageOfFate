using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Items;

namespace VillageOfFate.Services.DALServices.Core;

[RegisterApiService]
public class ItemDefinitionService(DataContext context) {
	public async Task<IEnumerable<ItemDefinitionDto>> GetItemDefinitionsWithoutImagesAsync() {
		return await context.ItemDefinitions.Where(i => i.Image.Base64Image == null).ToListAsync();
	}

	public async Task<ItemDefinitionDto> GetOrCreateBiblePageAsync() {
		var page = await context.ItemDefinitions.FirstOrDefaultAsync(p => p.Category == ItemCategory.Page && p.Name == "Bible Page");
		if (page != null) return page;

		page = new ItemDefinitionDto {
			Name = "Bible Page",
			Description = "A page from the God of Fate's Bible. It is said that the God of Fate will write new Divine Messages on these pages.",
			Edible = false,
			HungerRestored = 0,
			Category = ItemCategory.Page,
			Image = new ImageDto()
		};
		var entry = await context.AddAsync(page);
		await context.SaveChangesAsync();
		return entry.Entity;
	}
}