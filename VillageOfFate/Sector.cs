using System.Collections.Generic;
using System.Drawing;

namespace VillageOfFate;

public class Sector(Point position) {
	public Point Position { get; } = position;
	public string Description { get; set; } = string.Empty;
	public List<Item> Items { get; } = [];
}