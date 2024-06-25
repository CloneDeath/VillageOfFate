using System.Collections.Generic;
using System.Linq;
using VillageOfFate.Actions;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public class ActionFactory(
	AdjustEmotionalStateAction adjustEmotionalState,
	EatAction eat,
	IdleAction idle,
	InteractAction interact,
	LookoutAction lookout,
	SleepAction sleep,
	SpeakAction speak
) {
	public IReadOnlyList<IAction> Actions => [adjustEmotionalState, eat, idle, interact, lookout, sleep, speak];

	public IAction? Get(string actionName) => Actions.FirstOrDefault(a => a.Name == actionName);
	public IAction Get(ActivityName activityName) => Actions.First(a => a.ActivityName == activityName);
}