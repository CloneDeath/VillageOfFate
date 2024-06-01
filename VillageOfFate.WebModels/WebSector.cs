using System.Collections.Generic;

namespace VillageOfFate.WebModels;

public class WebSector(Position position) {
	public Position Position { get; } = position;
	public string Description { get; set; } = string.Empty;
	public List<WebItem> Items { get; init; } = [];
}