using Microsoft.AspNetCore.Mvc;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldController(World world) : ControllerBase {
	[HttpGet]
	public WebWorld GetWorld() => world.AsWebWorld();
}