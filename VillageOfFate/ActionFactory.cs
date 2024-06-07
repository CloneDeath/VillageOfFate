using System.Collections.Generic;
using System.Linq;
using VillageOfFate.Actions;

namespace VillageOfFate;

public class ActionFactory(IdleAction idle) {
	// private readonly IReadOnlyList<IVillagerAction> actions = [
	// 	new SpeakAction(logger),
	// 	new InteractAction(logger),
	// 	new AdjustEmotionalStateAction(logger),
	// 	new EatAction(logger),
	// 	new SleepAction(logger),
	// 	new LookoutAction(logger)
	// ];

	public IReadOnlyList<IAction> Actions => [idle];

	public IAction Get(string actionName) => Actions.First(a => a.Name == actionName);
}