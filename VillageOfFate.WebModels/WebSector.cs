using System;
using System.Collections.Generic;

namespace VillageOfFate.WebModels;

public class WebSector(Position position) {
	public required Guid Id { get; init; }
	public Position Position { get; } = position;
	public string Description { get; set; } = string.Empty;
	public List<WebItem> Items { get; init; } = [];
	public required Guid ImageId { get; init; }
}