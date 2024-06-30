using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class SpeakActivityDto() : ActivityDto(ActivityName.Speak) {
	public string Content { get; set; } = string.Empty;
}