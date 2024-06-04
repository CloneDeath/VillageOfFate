using System;
using System.Collections.Generic;
using VillageOfFate.DAL.Entities;
using VillageOfFate.Legacy;
using VillageOfFate.Legacy.VillagerActions;

namespace VillageOfFate;

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

	public IActivity Get(ActivityDto activity) => throw new NotImplementedException();
}