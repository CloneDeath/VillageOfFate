using System;
using System.Collections.Generic;
using System.Linq;
using VillageOfFate.WebModels;

namespace VillageOfFate.Legacy;

public class World {
	private readonly List<Villager> _villagers = [];
	public readonly List<WebSector> Sectors = [];
	public DateTime CurrenTime { get; set; } = DateTime.Now;
	public IEnumerable<Villager> Villagers => _villagers;

	public WebSector GetSector(Position position) {
		return Sectors.First(s => s.Position == position);
	}

	public IEnumerable<Villager> GetVillagersInSector(Position point) {
		return _villagers.Where(v => v.SectorLocation == point);
	}
}