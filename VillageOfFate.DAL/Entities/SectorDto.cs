using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities;

public class SectorDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public int X { get; set; }
	public int Y { get; set; }
	public Position Position => new(X, Y);

	public Guid? ImageId { get; set; }
	public ImageDto? Image { get; set; }

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Description { get; set; } = string.Empty;

	public List<ItemDto> Items { get; set; } = [];
	public List<VillagerDto> Villagers { get; set; } = [];
}