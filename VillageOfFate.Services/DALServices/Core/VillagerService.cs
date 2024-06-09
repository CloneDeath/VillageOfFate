using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerService(DataContext context) {
	public async Task<VillagerDto> CreateAsync(VillagerDto villager) {
		var result = await context.Villagers.AddAsync(villager);
		await context.SaveChangesAsync();
		return result.Entity;
	}

	public VillagerDto GetVillagerWithTheShortestCompleteTime() {
		return context.Villagers
					  .Include(v => v.Items)
					  .Include(v => v.Activities)
					  .Include(v => v.Sector)
					  .ToList()
					  .OrderBy(v => v.CurrentActivity != null
										? v.CurrentActivity.StartTime + v.CurrentActivity.Duration
										: DateTime.MaxValue).First();
	}

	public async Task<IEnumerable<VillagerDto>> GetAll() => await context.Villagers
																		 .Include(v => v.Items)
																		 .Include(v => v.Activities)
																		 .Include(v => v.Sector)
																		 .ToListAsync();

	public async Task<VillagerDto> Get(Guid id) => await context.Villagers
																.Include(v => v.Items)
																.Include(v => v.Activities)
																.Include(v => v.Sector)
																.FirstAsync(v => v.Id == id);

	public IEnumerable<VillagerDto> GetVillagersWithoutImages() {
		return context.Villagers.Where(v => v.ImageId == null);
	}
}