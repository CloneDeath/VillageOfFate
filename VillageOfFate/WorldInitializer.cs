using System.Collections.Generic;
using System.Threading.Tasks;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public class WorldInitializer(
	TimeService time,
	SectorService sectors
) {
	public async Task PopulateWorldAsync() {
		if (await sectors.SectorExistsAsync(Position.Zero)) return;

		await time.GetAsync(TimeLabel.World);
		await PopulateSector();
	}

	private async Task PopulateSector() {
		var sector = await sectors.GetOrCreateSectorAsync(Position.Zero, sector => {
			sector.Description =
				"A dense, lush forest filled with towering trees, diverse wildlife, and the sounds of nature. " +
				"It's easy to lose one's way in this vast sea of green.";
		});
		await sectors.AddItemRangeToSectorAsync(sector, new List<ItemDto> {
			new() {
				Quantity = 3,
				Definition = new ItemDefinitionDto {
					Name = "Ration",
					Description = "A small bag of dried fruit and nuts.",
					Edible = true, HungerRestored = 2,
					Image = new ImageDto()
				}
			},
			new() {
				Quantity = 1,
				Definition = new ItemDefinitionDto {
					Name = "Apple",
					Description = "A juicy, red apple. Perfect for a quick snack.",
					Edible = true, HungerRestored = 5,
					Image = new ImageDto()
				}
			},
			new() {
				Quantity = 1,
				Definition = new ItemDefinitionDto {
					Name = "Mushroom",
					Description = "A common forest mushroom. Make sure it's not poisonous before eating!",
					Edible = true, HungerRestored = 3,
					Image = new ImageDto()
				}
			},
			new() {
				Quantity = 1,
				Definition = new ItemDefinitionDto {
					Name = "Berries",
					Description = "A handful of wild berries. Sweet and nutritious.",
					Edible = true, HungerRestored = 4,
					Image = new ImageDto()
				}
			}
		});
	}
}