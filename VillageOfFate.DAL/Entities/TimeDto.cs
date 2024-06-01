using System;
using Microsoft.EntityFrameworkCore;

namespace VillageOfFate.DAL.Entities;

[PrimaryKey("Label")]
public class TimeDto {
	public TimeLabel Label { get; set; }
	public DateTime Now { get; set; }
}

public enum TimeLabel {
	World,
	End
}