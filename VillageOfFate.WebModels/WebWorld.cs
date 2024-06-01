using System;
using System.Collections.Generic;
using System.Linq;

namespace VillageOfFate.WebModels;

public class WebWorld {
	public List<WebSector> Sectors { get; init; } = [];
	public DateTime CurrenTime { get; set; } = DateTime.Now;
	public IEnumerable<WebVillager> Villagers { get; init; } = [];

	public WebSector GetSector(Position position) {
		return Sectors.First(s => s.Position == position);
	}
}