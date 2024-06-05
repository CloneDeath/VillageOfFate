using System;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL.Attributes;

namespace VillageOfFate.DAL.Entities;

[PrimaryKey("Label")]
public class TimeDto {
	public TimeLabel Label { get; set; }
	[UtcDateTime] public DateTime Time { get; set; }
}

public enum TimeLabel {
	World,
	End
}