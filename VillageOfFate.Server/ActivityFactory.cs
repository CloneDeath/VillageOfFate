using System;
using System.Collections.Generic;
using VillageOfFate.Activities;
using VillageOfFate.DAL.Entities;
using VillageOfFate.VillagerActions;

namespace VillageOfFate.Server;

public class ActivityFactory(VillageLogger logger) {
	private readonly IReadOnlyList<IVillagerAction> actions = [
		new SpeakAction(logger),
		new DoNothingAction(),
		new InteractAction(logger),
		new AdjustEmotionalStateAction(logger),
		new EatAction(logger),
		new SleepAction(logger),
		new LookoutAction(logger)
	];

	public Activity Get(ActivityDto villagerActivity) => throw new NotImplementedException();
}