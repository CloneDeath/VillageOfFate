using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController(ItemService items) : ControllerBase {
	[HttpGet("{id:guid}")]
	public async Task<WebItem> GetItem(Guid id) => (await items.GetAsync(id)).AsWebItem();
}