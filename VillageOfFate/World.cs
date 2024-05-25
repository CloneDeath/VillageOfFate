using System;
using System.Collections.Generic;
using System.Linq;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public class World {
	private readonly List<Sector> _sectors = [];
	private readonly List<Villager> _villagers = [];
	public DateTime CurrenTime { get; set; } = DateTime.Now;
	public IEnumerable<Villager> Villagers => _villagers;

	public Sector CreateSector(Position position) {
		var sector = new Sector(position);
		_sectors.Add(sector);
		return sector;
	}

	public Sector GetSector(Position position) {
		return _sectors.First(s => s.Position == position);
	}

	public void AddVillager(Villager villager) {
		_villagers.Add(villager);
	}

	public IEnumerable<Villager> GetVillagersInSector(Position point) {
		return _villagers.Where(v => v.SectorLocation == point);
	}
}