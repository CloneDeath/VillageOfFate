using VillageOfFate.WebModels;

namespace VillageOfFate.DAL.Entities.Activities;

public class IdleActivityDto : ActivityDto {
	public IdleActivityDto() {
		Name = ActivityName.Idle;
	}
}