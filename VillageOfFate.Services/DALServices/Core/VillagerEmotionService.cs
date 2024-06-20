using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.WebModels;

namespace VillageOfFate.Services.DALServices.Core;

public class VillagerEmotionService(DataContext context) {
	public async Task AdjustEmotionAsync(VillagerDto villager, VillagerEmotion emotion, int adjustment) {
		villager = context.Villagers.Entry(villager).Entity;
		villager.Emotions[emotion] += adjustment;
		await context.SaveChangesAsync();
	}
}