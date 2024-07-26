using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class VillagerActionErrorService(DataContext context, TimeService time) {
	public async Task LogInvalidAction(VillagerDto villager, string name, string arguments) {
		await context.VillagerActionErrors.AddAsync(new VillagerActionErrorDto {
			Villager = villager,
			ActionName = name,
			Arguments = arguments,
			Error = "Invalid Action",
			EarthTime = DateTime.UtcNow,
			WorldTime = await time.GetAsync(TimeLabel.World)
		});
		await context.SaveChangesAsync();
	}

	public async Task LogActionParseError(VillagerDto villager, string name, string arguments, Exception ex) {
		await context.VillagerActionErrors.AddAsync(new VillagerActionErrorDto {
			Villager = villager,
			ActionName = name,
			Arguments = arguments,
			Error = ex.Message,
			EarthTime = DateTime.UtcNow,
			WorldTime = await time.GetAsync(TimeLabel.World)
		});
		await context.SaveChangesAsync();
	}

	public async Task LogActionBeginError(VillagerDto villager, ActivityDto activity, Exception ex) {
		await context.VillagerActionErrors.AddAsync(new VillagerActionErrorDto {
			Villager = villager,
			ActionName = activity.Name.ToString(),
			Arguments = activity.Arguments,
			ActivityId = activity.Id,
			Error = ex.Message,
			EarthTime = DateTime.UtcNow,
			WorldTime = await time.GetAsync(TimeLabel.World)
		});
		await context.SaveChangesAsync();
	}
}