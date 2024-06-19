using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

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
								 ? v.CurrentActivity.StartTime + v.CurrentActivity.Duration
								 : DateTime.MaxValue).First();
	}

	public async Task<IEnumerable<VillagerDto>> GetAll() => await Villagers
																.ToListAsync();

	public async Task<VillagerDto> Get(Guid id) => await Villagers
													   .FirstAsync(v => v.Id == id);

	public async Task<IEnumerable<VillagerDto>> GetVillagersWithoutImages() {
		return await context.Villagers.Where(v => v.ImageId == null).ToListAsync();
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
}