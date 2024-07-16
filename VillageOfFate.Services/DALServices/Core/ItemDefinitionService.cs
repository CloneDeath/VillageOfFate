using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Items;

namespace VillageOfFate.Services.DALServices.Core;

[RegisterApiService]
public class ItemDefinitionService(DataContext context) {
	public async Task<IEnumerable<ItemDefinitionDto>> GetItemDefinitionsWithoutImagesAsync() {
		return await context.ItemDefinitions.Where(i => i.Image.Base64Image == null).ToListAsync();
	}
}