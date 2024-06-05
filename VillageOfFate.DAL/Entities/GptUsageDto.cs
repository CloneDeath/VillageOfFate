using System;
using VillageOfFate.DAL.Attributes;

namespace VillageOfFate.DAL.Entities;

public class GptUsageDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	[UtcDateTime] public DateTime WorldTime { get; set; }
	[UtcDateTime] public DateTime EarthTime { get; set; }
	public int TotalTokens { get; set; }
	public int PromptTokens { get; set; }
	public int CompletionTokens { get; set; }
}