using System;

namespace VillageOfFate.DAL.Entities;

public class TimeDto {
	public TimeLabel Label { get; set; }
	public DateTime Now { get; set; }
}

public enum TimeLabel {
	World,
	End
}