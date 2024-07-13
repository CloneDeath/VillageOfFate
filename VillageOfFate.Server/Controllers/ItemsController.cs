using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]/{id:guid}")]
public class ItemsController(ItemService items) : ControllerBase {
	[HttpGet]
	public async Task<WebItem> GetItem(Guid id) => (await items.GetAsync(id)).AsWebItem();

	[HttpGet("Location")]
	public async Task<WebItemLocation> GetItemLocation(Guid id) {
		var item = await items.GetItemLocationAsync(id);
		return new WebItemLocation {
			Villager = item.Villager == null
						   ? null
						   : new WebVillagerLocation {
							   Id = item.Villager.Id,
							   Sector = new WebSectorLocation {
								   Id = item.Villager.SectorId,
								   Position = item.Villager.Sector.Position
							   }
						   },
			Sector = item.Sector == null
						 ? null
						 : new WebSectorLocation {
							 Id = item.Sector.Id,
							 Position = item.Sector.Position
						 }
		};
	}
}