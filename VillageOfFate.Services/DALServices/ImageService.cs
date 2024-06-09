using Microsoft.EntityFrameworkCore;
using OpenAi;
using OpenAi.Images;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class ImageService(DataContext context, OpenApi api) {
	public async Task<ImageDto> GetAsync(Guid id) => await context.Images.FirstAsync(img => img.Id == id);

	public async Task<ImageDto> GenerateImageAsync(string prompt) {
		var response = await api.GenerateImageAsync(prompt, ImageSize._256x256, ResponseFormat.Base64_Json);
		var data = response.Data.First();
		var image = await context.Images.AddAsync(new ImageDto {
			Created = new DateTime(response.Created),
			Prompt = data.revised_prompt ?? prompt,
			Base64Image = data.b64_json ?? throw new InvalidDataException()
		});
		await context.SaveChangesAsync();
		return image.Entity;
	}

	public async Task GenerateImageFor(ItemDto item) {
		if (item.ImageId != null) return;
		item.Image = await GenerateImageAsync($"{item.Name}, {item.Description}");
		await context.SaveChangesAsync();
	}

	public async Task GenerateImageFor(SectorDto sector) {
		if (sector.ImageId != null) return;
		sector.Image = await GenerateImageAsync("Generate a top-down view of the sector with this description: "
												+ $"{sector.Description}");
		await context.SaveChangesAsync();
	}

	public async Task GenerateImageFor(VillagerDto villager) {
		if (villager.ImageId != null) return;
		villager.Image = await GenerateImageAsync($"Generate a portrait for the villager {villager.Name}: "
												  + $"{villager.GetDescription()}");
		await context.SaveChangesAsync();
	}
}