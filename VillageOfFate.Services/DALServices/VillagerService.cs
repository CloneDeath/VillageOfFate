using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class VillagerService(DataContext context) {
	public async Task<VillagerDto> CreateAsync(VillagerDto villager) {
		var result = await context.Villagers.AddAsync(villager);
		await context.SaveChangesAsync();
		return result.Entity;
	}

	public async Task<VillagerDto> GetVillagerWithTheShortestCompleteTime() {
		return await context.Villagers.OrderBy(v => v.Activity.StartTime + v.Activity.Duration).FirstAsync();
	}
}