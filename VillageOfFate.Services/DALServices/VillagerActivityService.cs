using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class VillagerActivityService(DataContext context, TimeService time) {
	public async Task PopAsync(VillagerDto villager) {
		villager = context.Villagers.Entry(villager).Entity;
		if (villager.CurrentActivity == null) return;
		context.Activities.Remove(villager.CurrentActivity);

		var newActivity = villager.ActivityQueue.FirstOrDefault();
		if (newActivity != null) {
			newActivity.StartTime = await time.GetAsync(TimeLabel.World);
		}

		await context.SaveChangesAsync();
	}

	public async Task AddAsync(VillagerDto villager, ActivityDto activityDetail) {
		villager = context.Villagers.Entry(villager).Entity;
		villager.Activities.Add(activityDetail);
		await context.SaveChangesAsync();
	}

	public async Task PushCurrentActivityIntoQueue(VillagerDto villager) {
		villager = context.Villagers.Entry(villager).Entity;

		var current = villager.CurrentActivity;
		if (current == null) return;

		var remainingTime = current.EndTime - await time.GetAsync(TimeLabel.World);
		current.Duration = remainingTime < TimeSpan.Zero ? TimeSpan.Zero : remainingTime;

		foreach (var activity in villager.ActivityQueue) {
			activity.StartTime += TimeSpan.FromDays(1);
		}

		await context.SaveChangesAsync();
	}
}