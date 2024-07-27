using System;

namespace VillageOfFate.WebModels;

public enum ActivityName {
	AdjustEmotionalState,
	Eat,
	Idle,
	Interact,
	Lookout,
	Sleep,
	Speak,
	Read
}

public static class ActivityNameExtensions {
	public static string ToActiveString(this ActivityName activityName) {
		return activityName switch {
			ActivityName.AdjustEmotionalState => "Adjusting Emotional State",
			ActivityName.Eat => "Eating",
			ActivityName.Idle => "Doing Nothing",
			ActivityName.Interact => "Interacting",
			ActivityName.Lookout => "On Lookout",
			ActivityName.Sleep => "Sleeping",
			ActivityName.Speak => "Speaking",
			ActivityName.Read => "Reading",
			_ => throw new ArgumentOutOfRangeException(nameof(activityName), activityName, null)
		};
	}

	public static string ToFutureString(this ActivityName activityName) {
		return activityName switch {
			ActivityName.AdjustEmotionalState => "Adjust Emotional State",
			ActivityName.Eat => "Eat",
			ActivityName.Idle => "Do Nothing",
			ActivityName.Interact => "Interact",
			ActivityName.Lookout => "Go on Lookout",
			ActivityName.Sleep => "Sleep",
			ActivityName.Speak => "Speak",
			ActivityName.Read => "Read",
			_ => throw new ArgumentOutOfRangeException(nameof(activityName), activityName, null)
		};
	}
}