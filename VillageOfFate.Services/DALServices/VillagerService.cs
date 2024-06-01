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

	public VillagerDto GetVillagerWithTheShortestCompleteTime() {
		return context.Villagers.Include(villagerDto => villagerDto.Activity).ToList()
					  .OrderBy(v => v.Activity.StartTime + v.Activity.Duration).First();
	}

	public async Task<IEnumerable<VillagerDto>> GetAll() => await context.Villagers.ToListAsync();
	public async Task<VillagerDto> Get(Guid id) => await context.Villagers.FirstAsync(v => v.Id == id);
}