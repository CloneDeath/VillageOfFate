using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class VillagersController(World world) : ControllerBase {
	[HttpGet]
	public IEnumerable<WebVillager> ListVillagers() => world.Villagers.Select(AsWebVillager);

	[HttpGet("{id:guid}")]
	public WebVillager GetVillager(Guid id) => AsWebVillager(world.Villagers.First(v => v.Id == id));

	private static WebVillager AsWebVillager(Villager villager) {
		return new WebVillager {
			Id = villager.Id,
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