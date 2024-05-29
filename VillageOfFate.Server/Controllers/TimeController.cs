using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController(TimeService time) : ControllerBase {
	[HttpGet]
	public async Task<DateTime> GetCurrentTime() => await time.GetTime();
}