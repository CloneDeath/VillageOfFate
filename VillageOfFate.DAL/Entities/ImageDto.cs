using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Attributes;

namespace VillageOfFate.DAL.Entities;

public class ImageDto {
	public const int MaxBase64ImageLength = 4_000_000;

	public Guid Id { get; set; } = Guid.NewGuid();
	[UtcDateTime] public DateTime Created { get; set; } = DateTime.UtcNow;
	[MaxLength(MaxBase64ImageLength)] public string Base64Image { get; set; } = string.Empty;
	[MaxLength(InitialCreate.MaxDescriptionLength)]
	public string Prompt { get; set; } = string.Empty;
}