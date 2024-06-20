using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public class WorldInitializer(
	TimeService time,
	SectorService sectors,
	VillagerService villagers,
	RelationshipService relations,
	VillagerItemService villagerItems,
	VillagerMemoryService villagerMemories,
	RandomProvider random
) {
	public async Task PopulateWorldAsync() {
		if (await sectors.SectorExistsAsync(Position.Zero)) {
			return;
		}

		await time.GetAsync(TimeLabel.World);
		var sector = await PopulateSector();
		await PopulateVillagers(sector);
	}

	private async Task<SectorDto> PopulateSector() {
		var sector = await sectors.GetOrCreateSectorAsync(Position.Zero, sector => {
			sector.Description =
				"A dense, lush forest filled with towering trees, diverse wildlife, and the sounds of nature. " +
				"It's easy to lose one's way in this vast sea of green.";
		});
		await sectors.AddItemRangeToSectorAsync(sector, new List<ItemDto> {
			new() {
				Name = "Ration",
				Description = "A small bag of dried fruit and nuts.",
				Quantity = 3, Edible = true, HungerRestored = 2
			},
			new() {
				Name = "Apple",
				Description = "A juicy, red apple. Perfect for a quick snack.",
				Quantity = 1, Edible = true, HungerRestored = 5
			},
			new() {
				Name = "Mushroom",
				Description = "A common forest mushroom. Make sure it's not poisonous before eating!",
				Quantity = 1, Edible = true, HungerRestored = 3
			},
			new() {
				Name = "Berries",
				Description = "A handful of wild berries. Sweet and nutritious.",
				Quantity = 1, Edible = true, HungerRestored = 4
			}
		});
		return sector;
	}

	private async Task PopulateVillagers(SectorDto sector) {
		var startTime = await time.GetAsync(TimeLabel.World);
		var maxIdle = TimeSpan.FromSeconds(5);
		var gamz = await villagers.CreateAsync(new VillagerDto {
			Name = "Gamz", Age = 26, Gender = Gender.Male,
			Summary = "Chemm's big brother. A warrior monk with multiple wounds on both his face and body.",
			Sector = sector,
			Hunger = 6,
			Activities = [
				new IdleActivityDto(random.NextTimeSpan(maxIdle)) {
					StartTime = startTime
				}
			]
		});
		await villagerItems.AddAsync(gamz, new ItemDto {
			Name = "Sword",
			Description = "A well-crafted sword with a leather-wrapped hilt.",
			Quantity = 1,
			Edible = false, HungerRestored = 0
		});

		var chem = await villagers.CreateAsync(new VillagerDto {
			Name = "Chemm", Age = 19, Gender = Gender.Female,
			Summary = "Gamz's little sister. A priestess who believes in the god of fate.",
			Sector = sector,
			Hunger = 5,
			Activities = [
				new IdleActivityDto(random.NextTimeSpan(maxIdle)) {
					StartTime = startTime
				}
			]
		});
		var carol = await villagers.CreateAsync(new VillagerDto {
			Name = "Carol", Age = 7, Gender = Gender.Female,
			Summary = "A cheerful child, although quite mature for her age.",
			Sector = sector,
			Hunger = 8,
			Activities = [
				new IdleActivityDto(random.NextTimeSpan(maxIdle)) {
					StartTime = startTime
				}
			]
		});
		var lyra = await villagers.CreateAsync(new VillagerDto {
			Name = "Lyra", Age = 30, Gender = Gender.Female,
			Summary = "A younger wife than her husband, but capable of keeping him in check.",
			Sector = sector,
			Hunger = 4,
			Activities = [
				new IdleActivityDto(random.NextTimeSpan(maxIdle)) {
					StartTime = startTime
				}
			]
		});
		var lodis = await villagers.CreateAsync(new VillagerDto {
			Name = "Lodis", Age = 33, Gender = Gender.Male,
			Summary = "The father of a family of three that ran a general store in the village.",
			Sector = sector,
			Hunger = 5,
			Activities = [
				new IdleActivityDto(random.NextTimeSpan(maxIdle)) {
					StartTime = startTime
				}
			]
		});

		await relations.AddRelationAsync(gamz, chem, "Younger Sister");
		await relations.AddRelationAsync(gamz, carol, "Child of Neighbors");
		await relations.AddRelationAsync(gamz, lyra, "Neighbor");
		await relations.AddRelationAsync(gamz, lodis, "Neighbor");

		await relations.AddRelationAsync(chem, gamz, "Older Brother");
		await relations.AddRelationAsync(chem, carol, "Friend");
		await relations.AddRelationAsync(chem, lyra, "Neighbor");
		await relations.AddRelationAsync(chem, lodis, "Neighbor");

		await relations.AddRelationAsync(carol, gamz, "Neighbor");
		await relations.AddRelationAsync(carol, chem, "Friend");
		await relations.AddRelationAsync(carol, lyra, "Mom");
		await relations.AddRelationAsync(carol, lodis, "Dad");

		await relations.AddRelationAsync(lyra, gamz, "Neighbor");
		await relations.AddRelationAsync(lyra, chem, "Neighbor");
		await relations.AddRelationAsync(lyra, carol, "Daughter");
		await relations.AddRelationAsync(lyra, lodis, "Husband");

		await relations.AddRelationAsync(lodis, gamz, "Neighbor");
		await relations.AddRelationAsync(lodis, chem, "Neighbor");
		await relations.AddRelationAsync(lodis, carol, "Daughter");
		await relations.AddRelationAsync(lodis, lyra, "Wife");

		var villagerDTOs = new[] { gamz, chem, carol, lyra, lodis };
		foreach (var villager in villagerDTOs) {
			await villagerMemories.AddAsync(villager,
				$"You and {villagerDTOs.Length - 1} other villagers are lost in the woods, "
				+ "having just escaped a goblin attack that destroyed your home and entire village.");
		}
	}
}