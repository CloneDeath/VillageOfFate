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
		return context.Villagers.Include(villagerDto => villagerDto.Activities).ToList()
					  .Where(v => v.CurrentActivity != null)
					  .OrderBy(v => v.CurrentActivity.StartTime + v.CurrentActivity.Duration).First();
	}

	public async Task<IEnumerable<VillagerDto>> GetAll() => await context.Villagers
																		 .Include(v => v.Items)
																		 .Include(v => v.Activities)
																		 .Include(v => v.Sector)
																		 .ToListAsync();

	public async Task<VillagerDto> Get(Guid id) => await context.Villagers
																.Include(v => v.Items)
																.Include(v => v.CurrentActivity)
																.Include(v => v.Sector)
																.FirstAsync(v => v.Id == id);
}