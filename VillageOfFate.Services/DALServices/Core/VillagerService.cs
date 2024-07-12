using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerService(DataContext context) {
	protected IQueryable<VillagerDto> Villagers => context.Villagers
														  .Include(v => v.Items)
														  .Include(v => v.Activities)
														  .Include(v => v.Sector);

	public async Task<VillagerDto> CreateAsync(VillagerDto villager) {
		var result = await context.Villagers.AddAsync(villager);
		await context.SaveChangesAsync();
		return result.Entity;
	}

	public VillagerDto GetVillagerWithTheEarliestCompleteTime() {
		return Villagers
			   .ToList()
			   .OrderBy(v => v.CurrentActivity != null
								 ? v.CurrentActivity.StartTime + v.CurrentActivity.DurationRemaining
								 : DateTime.MaxValue).First();
	}

	public async Task<IEnumerable<VillagerDto>> GetAllAsync() => await Villagers
																	 .ToListAsync();

	public async Task<VillagerDto> GetAsync(Guid id) => await Villagers
															.FirstAsync(v => v.Id == id);

	public async Task<IEnumerable<VillagerDto>> GetManyAsync(IEnumerable<VillagerDto> villagers) {
		var ids = villagers.Select(v => v.Id).ToArray();
		return await GetManyAsync(ids);
	}

	public async Task<IEnumerable<VillagerDto>> GetManyAsync(Guid[] villagerIds) {
		return await Villagers
					 .Where(v => villagerIds.Contains(v.Id))
					 .ToListAsync();
	}

	public async Task<IEnumerable<VillagerDto>> GetVillagersWithoutImages() {
		return await context.Villagers.Where(v => v.Image.Base64Image == null).ToListAsync();
	}

	public async Task<int> GetVillagerCountAsync(Guid id) {
		return await context.Villagers.CountAsync(v => v.SectorId == id);
	}

	public async Task<IEnumerable<VillagerDto>> GetVillagersInSectorAsync(Guid sectorId) {
		return await Villagers.Where(v => v.SectorId == sectorId).ToListAsync();
	}

	public async Task<IEnumerable<VillagerDto>> GetVillagersWithoutActivities() {
		return await Villagers.Where(v => v.Activities.Count == 0).ToListAsync();
	}

	public async Task DecreaseHungerAsync(VillagerDto villager, int amount) {
		villager = context.Villagers.Entry(villager).Entity;
		villager.Hunger = Math.Max(0, villager.Hunger - amount);
		await context.SaveChangesAsync();
	}
}