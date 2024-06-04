using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class VillagerActivityService(DataContext context, TimeService time) {
	public async Task PopAsync(VillagerDto villager) {
		villager = context.Villagers.Entry(villager).Entity;
		context.Activities.Remove(villager.CurrentActivity);

		var newActivity = villager.ActivityQueue.First();
		villager.CurrentActivity = newActivity;
		newActivity.StartTime = await time.GetAsync(TimeLabel.World);

		await context.SaveChangesAsync();
	}
}