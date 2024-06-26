using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class VillagerActivityService(DataContext context, TimeService time) {
	public async Task PopAsync(VillagerDto villager) {
		villager = context.Villagers.Entry(villager).Entity;
		if (villager.CurrentActivity == null) return;
		context.Activities.Remove(villager.CurrentActivity);

		var newActivity = villager.ActivityQueue.FirstOrDefault();
		if (newActivity != null) {
			var worldNow = await time.GetAsync(TimeLabel.World);
			newActivity.StartTime = newActivity.StartTime < worldNow ? worldNow : newActivity.StartTime;
		}

		await context.SaveChangesAsync();
	}

	public async Task AddAsync(VillagerDto villager, ActivityDto activity) {
		villager = context.Villagers.Entry(villager).Entity;
		activity.Villager = villager;

		var current = villager.CurrentActivity;
		if (current != null && current.Priority >= activity.Priority) {
			var worldNow = await time.GetAsync(TimeLabel.World);
			if (current.StartTime < worldNow) {
				var remainingTime = current.EndTime - worldNow;
				current.DurationRemaining = remainingTime < TimeSpan.Zero ? TimeSpan.Zero : remainingTime;
			}
		}

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
}