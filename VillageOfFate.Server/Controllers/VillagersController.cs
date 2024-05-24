using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class VillagersController : ControllerBase {
	[HttpGet]
	public IEnumerable<WebVillager> ListVillagers() {
		var random = new RandomProvider();
		var world = VillageOfFate.Program.GetInitialWorld();
		var villagers = VillageOfFate.Program.GetInitialVillagers(world, random);
		return villagers.Select(AsWebVillager);
	}

	private static WebVillager AsWebVillager(Villager villager) {
		return new WebVillager {
			Name = villager.Name,
			Gender = villager.Gender switch {
				Gender.Male => WebGender.Male,
				Gender.Female => WebGender.Female,
				_ => throw new Exception($"No matching value found for Gender.{villager.Gender}")
			},
			Summary = villager.Summary,
			Age = villager.Age
		};
	}
}