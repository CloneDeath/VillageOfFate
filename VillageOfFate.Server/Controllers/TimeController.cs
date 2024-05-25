using System;
using Microsoft.AspNetCore.Mvc;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController(World world) : ControllerBase {
	[HttpGet]
	public DateTime GetCurrentTime() => world.CurrenTime;
}