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
}