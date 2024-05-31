using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities;

public class VillagerMemoryDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid VillagerId { get; set; }
	public VillagerDto Villager { get; set; } = null!;

	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Memory { get; set; } = string.Empty;
}