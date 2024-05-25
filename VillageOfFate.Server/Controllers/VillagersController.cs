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
	public IEnumerable<WebVillager> ListVillagers() => world.Villagers.Select(v => v.AsWebVillager());

	[HttpGet("{id:guid}")]
	public WebVillager GetVillager(Guid id) => world.Villagers.First(v => v.Id == id).AsWebVillager();
}