using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerMemoryService(DataContext context) {
	public async Task AddAsync(VillagerDto villager, string memory) {
		await context.VillagerMemories.AddAsync(new VillagerMemoryDto {
			Memory = memory,
			Villager = villager
		});
		await context.SaveChangesAsync();
	}
}