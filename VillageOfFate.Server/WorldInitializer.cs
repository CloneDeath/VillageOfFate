using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageOfFate.Activities;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server;

public class WorldInitializer(SectorService sectors, VillagerService villagers) {
	public async Task PopulateWorldAsync() {
		if (await sectors.SectorExistsAsync(Position.Zero)) {
			return;
		}

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
		var maxIdle = TimeSpan.FromSeconds(5);
		var gamz = await villagers.CreateAsync(new VillagerDto {
			Name = "Gamz", Age = 26, Gender = Gender.Male,
			Summary = "Chemm's big brother. A warrior monk with multiple wounds on both his face and body.",
			SectorId = sector.Id,
			Hunger = 6,
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		});
		villagers.AddItem(gamz, new Item {
			Name = "Sword",
			Description = "A well-crafted sword with a leather-wrapped hilt.",
			Quantity = 1
		});

		var chem = await villagers.CreateAsync(new VillagerDto {
			Name = "Chemm", Age = 19, Gender = Gender.Female,
			Summary = "Gamz's little sister. A priestess who believes in the god of fate.",
			SectorId = sector.Id,
			Hunger = 5,
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		});
		var carol = await villagers.CreateAsync(new VillagerDto {
			Name = "Carol", Age = 7, Gender = Gender.Female,
			Summary = "A cheerful child, although quite mature for her age.",
			SectorId = sector.Id,
			Hunger = 8,
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		});
		var lyra = await villagers.CreateAsync(new VillagerDto {
			Name = "Lyra", Age = 30, Gender = Gender.Female,
			Summary = "A younger wife than her husband, but capable of keeping him in check.",
			SectorId = sector.Id,
			Hunger = 4,
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		});
		var lodis = await villagers.CreateAsync(new VillagerDto {
			Name = "Lodis", Age = 33, Gender = Gender.Male,
			Summary = "The father of a family of three that ran a general store in the village.",
			SectorId = sector.Id,
			Hunger = 5,
			CurrentActivity = new IdleActivity(random.NextTimeSpan(maxIdle), world)
		});

		gamz.AddRelationship(chem, "Younger Sister");
		gamz.AddRelationship(carol, "Child of Neighbors");
		gamz.AddRelationship(lyra, "Neighbor");
		gamz.AddRelationship(lodis, "Neighbor");

		chem.AddRelationship(gamz, "Older Brother");
		chem.AddRelationship(carol, "Friend");
		chem.AddRelationship(lyra, "Neighbor");
		chem.AddRelationship(lodis, "Neighbor");

		carol.AddRelationship(gamz, "Neighbor");
		carol.AddRelationship(chem, "Friend");
		carol.AddRelationship(lyra, "Mom");
		carol.AddRelationship(lodis, "Dad");

		lyra.AddRelationship(gamz, "Neighbor");
		lyra.AddRelationship(chem, "Neighbor");
		lyra.AddRelationship(carol, "Daughter");
		lyra.AddRelationship(lodis, "Husband");

		lodis.AddRelationship(gamz, "Neighbor");
		lodis.AddRelationship(chem, "Neighbor");
		lodis.AddRelationship(carol, "Daughter");
		lodis.AddRelationship(lyra, "Wife");

		//var villagers = new[] { gamz, chem, carol, lyra, lodis };
		foreach (var villager in villagers) {
			villager.AddMemory($"You and {villagers.Length - 1} other villagers are lost in the woods, "
							   + "having just escaped a goblin attack that destroyed your home and entire village.");
			world.AddVillager(villager);
		}

		return villagers;
	}
}