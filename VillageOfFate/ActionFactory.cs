using System.Collections.Generic;
using System.Linq;
using VillageOfFate.Actions;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public class ActionFactory(IdleAction idle, AdjustEmotionalStateAction adjustEmotionalState) {
	// private readonly IReadOnlyList<IVillagerAction> actions = [
	// 	new SpeakAction(logger),
	// 	new InteractAction(logger),
	// 	new EatAction(logger),
	// 	new SleepAction(logger),
	// 	new LookoutAction(logger)
	// ];

	public IReadOnlyList<IAction> Actions => [idle, adjustEmotionalState];

	public IAction? Get(string actionName) => Actions.FirstOrDefault(a => a.Name == actionName);
	public IAction Get(ActivityName activityName) => Actions.First(a => a.ActivityName == activityName);
}