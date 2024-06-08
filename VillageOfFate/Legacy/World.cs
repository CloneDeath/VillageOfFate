using System;
using System.Collections.Generic;
using VillageOfFate.WebModels;

namespace VillageOfFate.Legacy;

public class World {
	public DateTime CurrenTime { get; set; } = DateTime.Now;

	public WebSector GetSector(Position position) => throw new Exception();

	public IEnumerable<Villager> GetVillagersInSector(Position point) => [];
}