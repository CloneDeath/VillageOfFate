using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
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
	}
}