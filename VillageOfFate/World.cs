using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace VillageOfFate;

public class World {
	private readonly List<Sector> _sectors = [];
	private readonly List<Villager> _villagers = [];
	public DateTime CurrenTime { get; set; } = DateTime.Now;

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

	public IEnumerable<Villager> GetVillagersInSector(Point point) {
		return _villagers.Where(v => v.SectorLocation == point);
	}
}