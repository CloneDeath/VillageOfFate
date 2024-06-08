using OpenAi.Gpt;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Services.DALServices;

public class GptUsageService(DataContext context, TimeService time) {
	public async Task AddUsageAsync(ChatGptResponse response) {
		await context.GptUsage.AddAsync(new GptUsageDto {
			WorldTime = await time.GetAsync(TimeLabel.World),
			EarthTime = DateTime.UtcNow,
			CompletionTokens = response.Usage.CompletionTokens,
			PromptTokens = response.Usage.PromptTokens,
			TotalTokens = response.Usage.TotalTokens
		});
		await context.SaveChangesAsync();
	}
}