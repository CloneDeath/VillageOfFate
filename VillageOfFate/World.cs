using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace VillageOfFate;

public class World {
	private readonly List<Sector> _sectors = [];
	private readonly List<Villager> _villagers = [];

	public Sector CreateSector(Point position) {
		var sector = new Sector(position);
		_sectors.Add(sector);
		return sector;
	}

	public Sector GetSector(Point position) {
		return _sectors.First(s => s.Position == position);
	}

	public void AddVillager(Villager villager) {
		_villagers.Add(villager);
	}
}