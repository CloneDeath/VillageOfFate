using System.Collections.Generic;

namespace VillageOfFate.WebModels;

public class Sector(Position position) {
	public Position Position { get; } = position;
	public string Description { get; set; } = string.Empty;
	public List<Item> Items { get; } = [];
}