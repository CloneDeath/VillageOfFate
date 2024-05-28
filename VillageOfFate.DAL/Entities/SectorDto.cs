using System;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities;

public class SectorDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public int X { get; set; }
	public int Y { get; set; }
	public Position Position => new(X, Y);
	public string Description { get; set; } = string.Empty;
}