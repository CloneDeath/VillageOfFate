using Microsoft.EntityFrameworkCore;
using OpenAi;
using OpenAi.Images;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Villagers;

namespace VillageOfFate.Services.DALServices;

public class ImageService(DataContext context, OpenApi api) {
	public async Task<ImageDto> GetAsync(Guid id) => await context.Images.FirstAsync(img => img.Id == id);

	public async Task GenerateImageAsync(ImageDto image, string prompt) {
		var response = await api.GenerateImageAsync(prompt, ImageSize._256x256, ResponseFormat.Base64_Json);
		var data = response.Data.First();
		context.Images.Update(image);
		image.Created = new DateTime(response.Created);
		image.Prompt = data.revised_prompt ?? prompt;
		image.Base64Image = data.b64_json ?? throw new InvalidDataException();
		await context.SaveChangesAsync();
	}

	public async Task GenerateImageFor(ItemDto item) {
		await GenerateImageAsync(item.Image, $"Generate an image for this item, with a solid white background: " +
											 $"{item.Name}, {item.Description}");
	}

	public async Task GenerateImageFor(SectorDto sector) {
		await GenerateImageAsync(sector.Image, "Generate a top-down view of the sector with this description: "
											   + $"{sector.Description}");
	}

	public async Task GenerateImageFor(VillagerDto villager) {
		await GenerateImageAsync(villager.Image,
			$"Generate a photorealistic portrait for the villager {villager.Name}: "
			+ $"{villager.GetDescription()}");
	}
}