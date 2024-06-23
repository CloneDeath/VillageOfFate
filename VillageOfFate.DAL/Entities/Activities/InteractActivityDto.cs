namespace VillageOfFate.DAL.Entities.Activities;

public class InteractActivityDto : ActivityDto {
	public string[] Targets { get; set; } = [];
	public double DurationInSeconds { get; set; }
	public string Action { get; set; } = string.Empty;

}