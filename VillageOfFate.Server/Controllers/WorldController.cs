using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldController(World world, TimeService time) : ControllerBase {
	[HttpGet]
	public async Task<WebWorld> GetWorld() {
		var result = world.AsWebWorld();
		result.CurrenTime = await time.GetTimeAsync();
		return result;
	}
}