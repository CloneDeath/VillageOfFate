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
public class SectorsController(SectorService sectors) : ControllerBase {
	[HttpGet]
	public async Task<IEnumerable<WebSector>> GetAllAsync() =>
		(await sectors.GetAllAsync()).Select(s => s.AsWebSector());

	[HttpGet("{id:guid}")]
	public async Task<WebSector> GetSectorAsync(Guid id) => (await sectors.GetAsync(id)).AsWebSector();

	[HttpGet("{id:guid}/VillagerCount")]
	public async Task<int> GetSectorVillagerAccountAsync(Guid id) => await sectors.GetVillagerCountAsync(id);
}