using System;

namespace VillageOfFate.DAL.Entities.Activities;

public class IdleActivityDto : ActivityDto {
	public IdleActivityDto(TimeSpan duration) : this() {
		Duration = duration;
	}

	public IdleActivityDto() {
		Name = ActivityName.Idle;
		Description = "{villager} sits Idle";
		Interruptible = true;
	}
}