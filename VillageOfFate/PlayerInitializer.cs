using System;
using System.Threading.Tasks;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Items;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.BIServices;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public class PlayerInitializer(
	TimeService time,
	SectorService sectors,
	VillagerService villagers,
	RelationshipService relations,
	VillagerItemService villagerItems,
	EventsService events,
	UserService users
) {
	public async Task PopulatePlayerAsync(UserDto user) {
		if (user.Bible != null) return;

		var sector = await sectors.TryGetAsync(Position.Zero)
					 ?? throw new NullReferenceException("Sector Zero is not found");
		await PopulateVillagers(sector, user);
	}

	private async Task PopulateVillagers(SectorDto sector, UserDto user) {
		var gamz = await villagers.CreateAsync(new VillagerDto {
			Name = "Gamz", Age = 26, Gender = Gender.Male,
			Summary = "Chemm's big brother. A warrior monk with multiple wounds on both his face and body.",
			Sector = sector,
			Hunger = 6,
			Image = new ImageDto()
		});
		await villagerItems.AddAsync(gamz, new ItemDto {
			Quantity = 1,
			Definition = new ItemDefinitionDto {
				Name = "Sword",
				Description = "A well-crafted sword with a leather-wrapped hilt.",
				Edible = false, HungerRestored = 0,
				Image = new ImageDto()
			}
		});

		var chem = await villagers.CreateAsync(new VillagerDto {
			Name = "Chemm", Age = 19, Gender = Gender.Female,
			Summary = "Gamz's little sister. A priestess who believes in the god of fate.",
			Sector = sector,
			Hunger = 5,
			Image = new ImageDto()
		});
		user.Bible = await villagerItems.AddAsync(chem, new ItemDto {
			Quantity = 1,
			Definition = new ItemDefinitionDto {
				Name = "Holy Bible of the God of Fate",
				Description =
					"This Bible will glow whenever the God of Fate performs a Miracle or sends a Divine Message. " +
					"When the God of Fate sends a new Divine Message, you will feel it; As a Priestess, you should" +
					" read this Divine Message aloud for all other villagers to hear.",
				Edible = false, HungerRestored = 0,
				Image = new ImageDto()
			}
		});
		await users.SaveAsync(user);
		await villagerItems.AddAsync(chem, new ItemDto {
			Quantity = 1,
			Definition = new ItemDefinitionDto {
				Name = "Healing Potion",
				Description = "A small vial of red liquid that heals minor wounds.",
				Edible = false, HungerRestored = 0,
				Image = new ImageDto()
			}
		});

		var carol = await villagers.CreateAsync(new VillagerDto {
			Name = "Carol", Age = 7, Gender = Gender.Female,
			Summary = "A cheerful child, although quite mature for her age.",
			Sector = sector,
			Hunger = 8,
			Image = new ImageDto()
		});
		await villagerItems.AddAsync(carol, new ItemDto {
			Quantity = 1,
			Definition = new ItemDefinitionDto {
				Name = "Doll",
				Description = "A small, handmade doll with a stitched-on smile.",
				Edible = false, HungerRestored = 0,
				Image = new ImageDto()
			}
		});

		var lyra = await villagers.CreateAsync(new VillagerDto {
			Name = "Lyra", Age = 30, Gender = Gender.Female,
			Summary = "A younger wife than her husband, but capable of keeping him in check.",
			Sector = sector,
			Hunger = 4,
			Image = new ImageDto()
		});
		await villagerItems.AddAsync(lyra, new ItemDto {
			Quantity = 1,
			Definition = new ItemDefinitionDto {
				Name = "Lyra's Wedding Ring",
				Description = "A simple gold band that symbolizes the bond between you and your husband.",
				Edible = false, HungerRestored = 0,
				Image = new ImageDto()
			}
		});

		var lodis = await villagers.CreateAsync(new VillagerDto {
			Name = "Lodis", Age = 33, Gender = Gender.Male,
			Summary = "The father of a family of three that ran a general store in the village.",
			Sector = sector,
			Hunger = 5,
			Image = new ImageDto()
		});
		await villagerItems.AddAsync(lodis, new ItemDto {
			Quantity = 1,
			Definition = new ItemDefinitionDto {
				Name = "Lodis' Wedding Ring",
				Description = "A simple gold band that symbolizes the bond between you and your wife.",
				Edible = false, HungerRestored = 0,
				Image = new ImageDto()
			}
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

		var worldTime = await time.GetAsync(TimeLabel.World);

		var villagerDTOs = new[] { gamz, chem, carol, lyra, lodis };
		await events.AddAsync(sector, villagerDTOs,
			$"You and {villagerDTOs.Length - 1} other villagers are lost in the woods, "
			+ "having just escaped a goblin attack that destroyed your home and entire village.",
			worldTime - TimeSpan.FromHours(1));
	}
}