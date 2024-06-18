using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController(TimeService time) : ControllerBase {
	[HttpGet("World")]
	public async Task<DateTime> GetWorldTime() => await time.GetAsync(TimeLabel.World);

	[HttpGet("End")]
	public async Task<DateTime> GetEndTime() => await time.GetAsync(TimeLabel.End);
}