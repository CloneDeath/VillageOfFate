using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class VillagersController(VillagerService villagers) : ControllerBase {
	[HttpGet]
	public async Task<IEnumerable<WebVillager>> ListVillagers() {
		var results = await villagers.GetAll();
		return results.Select(v => v.AsWebVillager());
	}

	[HttpGet("{id:guid}")]
	public async Task<WebVillager> GetVillager(Guid id) => (await villagers.Get(id)).AsWebVillager();
}