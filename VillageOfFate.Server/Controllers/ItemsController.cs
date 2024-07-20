using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]/{itemId:guid}")]
public class ItemsController(ItemService items) : ControllerBase {
	[HttpGet]
	public async Task<WebItem> GetItem(Guid itemId) => (await items.GetAsync(itemId)).AsWebItem();

	[HttpGet("location")]
	public async Task<WebItemLocation> GetItemLocation(Guid itemId) {
		var item = await items.GetWithLocationAsync(itemId);
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

	[HttpGet("pages")]
	public async Task<IEnumerable<WebItem>> GetPagesInItem(Guid itemId) {
		var pages = await items.GetChildItemPagesAsync(itemId);
		return pages.Select(p => p.AsWebItem());
	}
}