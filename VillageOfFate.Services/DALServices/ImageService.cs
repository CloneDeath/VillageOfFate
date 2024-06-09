using Microsoft.EntityFrameworkCore;
using OpenAi;
using OpenAi.Images;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class ImageService(DataContext context, OpenApi api) {
	public async Task<ImageDto> GenerateImageAsync(string prompt) {
		var response = await api.GenerateImageAsync(prompt, ImageSize._256x256, ResponseFormat.Base64_Json);
		var data = response.Data.First();
		var image = await context.Images.AddAsync(new ImageDto {
			Created = new DateTime(response.Created),
			Prompt = data.revised_prompt ?? prompt,
			Base64Image = data.b64_json ?? throw new InvalidDataException()
		});
		return image.Entity;
	}

	public async Task<ImageDto> GetAsync(Guid id) => await context.Images.FirstAsync(img => img.Id == id);
}