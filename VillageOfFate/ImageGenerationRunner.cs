using System;
using System.Threading;
using System.Threading.Tasks;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate;

public class ImageGenerationRunner(
	VillagerService villagers,
	SectorService sectors,
	ItemService items,
	ImageService image
) {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task RunAsync(CancellationToken cancellationToken) {
		try {
			while (!cancellationToken.IsCancellationRequested) {
				await GenerateImages();
				await Task.Delay(Interval, cancellationToken);
			}
		}
		catch (OperationCanceledException) {
			Console.WriteLine("ImageGenerationRunner was cancelled");
		}
		catch (Exception e) {
			await Console.Error.WriteLineAsync($"ImageGenerationRunner threw an exception: {e}");
		}
		finally {
			Console.WriteLine("Exiting ImageGenerationRunner");
		}
	}

	private async Task GenerateImages() {
		await GenerateMissingVillagerImages();
		await GenerateMissingSectorImages();
		await GenerateMissingItemImages();
	}

	private async Task GenerateMissingVillagerImages() {
		foreach (var villager in villagers.GetVillagersWithoutImages()) {
			await image.GenerateImageFor(villager);
		}
	}

	private async Task GenerateMissingSectorImages() {
		foreach (var sector in sectors.GetSectorsWithoutImages()) {
			await image.GenerateImageFor(sector);
		}
	}

	private async Task GenerateMissingItemImages() {
		foreach (var item in items.GetItemsWithoutImages()) {
			await image.GenerateImageFor(item);
		}
	}
}