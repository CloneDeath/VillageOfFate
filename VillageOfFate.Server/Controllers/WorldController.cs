using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldController(TimeService time, VillagerService villagers, SectorService sectors) : ControllerBase {
	[HttpGet]
	public async Task<WebWorld> GetWorld() {
		return new WebWorld {
			CurrenTime = await time.GetAsync(TimeLabel.World),
			Villagers = (await villagers.GetAll()).Select(v => v.AsWebVillager()),
			Sectors = (await sectors.GetAll()).Select(s => s.AsWebSector()).ToList()
		};
	}
}