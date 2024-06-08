using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SectorController(SectorService sectors) : ControllerBase {
	[HttpGet("{id:guid}")]
	public async Task<WebSector> GetSectorAsync(Guid id) => (await sectors.Get(id)).AsWebSector();
}