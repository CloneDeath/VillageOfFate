using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]/{id:guid}")]
public class ItemsController(ItemService items) : ControllerBase {
	[HttpGet]
	public async Task<WebItem> GetItem(Guid id) => (await items.GetAsync(id)).AsWebItem();

	[HttpGet("Location")]
	public async Task<WebItemLocation> GetItemLocation(Guid id) {
		var item = await items.GetWithLocationAsync(id);
		return new WebItemLocation {
			Id = item.Id,
			Name = item.Definition.Name,
			Villager = item.Villager == null
						   ? null
						   : new WebVillagerLocation {
							   Id = item.Villager.Id,
							   Name = item.Villager.Name,
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
						 },
			Item = item.Item == null
					   ? null
					   : await GetItemLocation(item.Item.Id)
		};
	}
}