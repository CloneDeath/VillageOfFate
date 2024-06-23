using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class VillagersController(VillagerService villagers, EventsService events) : ControllerBase {
	[HttpGet]
	public async Task<IEnumerable<WebVillager>> ListVillagers() {
		var results = await villagers.GetAllAsync();
		return results.Select(v => v.AsWebVillager());
	}

	[HttpGet("{id:guid}")]
	public async Task<WebVillager> GetVillager(Guid id) => (await villagers.GetAsync(id)).AsWebVillager();

	[HttpGet("{id:guid}/Events")]
	public async Task<IEnumerable<WebEvent>> GetEvents(Guid id) =>
		(await events.GetVillagerEvents(id)).Select(e => e.AsWebEvent());
}