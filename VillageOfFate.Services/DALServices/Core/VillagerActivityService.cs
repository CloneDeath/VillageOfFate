using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerActivityService(DataContext context) {
	public async Task CompleteAsync(ActivityDto activity) {
		activity.Status = ActivityStatus.Complete;
		context.Activities.Update(activity);
		await context.SaveChangesAsync();
	}

	public async Task AddAsync(VillagerDto villager, ActivityDto activity) {
		villager = context.Villagers.Entry(villager).Entity;
		activity.Villager = villager;

		var lowerPriorityActivities = await context.Activities
												   .Where(a => a.Priority >= activity.Priority &&
															   a.VillagerId == villager.Id)
												   .ToListAsync();
		foreach (var lowPriorityActivity in lowerPriorityActivities) {
			lowPriorityActivity.Priority++;
		}

		context.Activities.UpdateRange(lowerPriorityActivities);

		await context.Activities.AddAsync(activity);
		await context.SaveChangesAsync();
	}

	public async Task SaveAsync(ActivityDto activity) {
		context.Activities.Update(activity);
		await context.SaveChangesAsync();
	}
}