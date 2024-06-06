using System;
using System.Collections.Generic;
using VillageOfFate.Legacy;

namespace VillageOfFate;

public class ActionFactory(VillageLogger logger) {
	// private readonly IReadOnlyList<IVillagerAction> actions = [
	// 	new SpeakAction(logger),
	// 	new DoNothingAction(),
	// 	new InteractAction(logger),
	// 	new AdjustEmotionalStateAction(logger),
	// 	new EatAction(logger),
	// 	new SleepAction(logger),
	// 	new LookoutAction(logger)
	// ];

	public IReadOnlyList<IAction> Actions => throw new NotImplementedException();

	public IAction Get(string actionName) => throw new NotImplementedException();
}