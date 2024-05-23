using System;
using Microsoft.AspNetCore.Mvc;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase {
	[HttpGet]
	public DateTime GetCurrentTime() => DateTime.Now;
}