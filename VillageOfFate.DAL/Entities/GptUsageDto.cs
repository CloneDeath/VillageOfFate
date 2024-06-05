using System;

namespace VillageOfFate.DAL.Entities;

public class GptUsageDto {
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTime WorldTime { get; set; }
	public DateTime EarthTime { get; set; }
	public int TotalTokens { get; set; }
	public int PromptTokens { get; set; }
	public int CompletionTokens { get; set; }
}