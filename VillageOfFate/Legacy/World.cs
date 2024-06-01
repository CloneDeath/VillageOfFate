using System;
using System.Collections.Generic;
using System.Linq;
using VillageOfFate.WebModels;

namespace VillageOfFate.Legacy;

public class World {
	private readonly List<Villager> _villagers = [];
	public readonly List<Sector> Sectors = [];
	public DateTime CurrenTime { get; set; } = DateTime.Now;
	public IEnumerable<Villager> Villagers => _villagers;

	public Sector CreateSector(Position position) {
		var sector = new Sector(position);
		Sectors.Add(sector);
		return sector;
	}

	public Sector GetSector(Position position) {
		return Sectors.First(s => s.Position == position);
	}

	public void AddVillager(Villager villager) {
		_villagers.Add(villager);
	}

	public IEnumerable<Villager> GetVillagersInSector(Position point) {
		return _villagers.Where(v => v.SectorLocation == point);
	}
}