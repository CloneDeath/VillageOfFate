using System;
using System.Collections.Generic;
using System.Linq;

namespace VillageOfFate.WebModels;

public class WebVillagerEmotions {
	public int Happiness { get; init; }
	public int Sadness { get; init; }
	public int Fear { get; init; }

	public int this[VillagerEmotion emotion] {
		get {
			return emotion switch {
				VillagerEmotion.Happiness => Happiness,
				VillagerEmotion.Sadness => Sadness,
				VillagerEmotion.Fear => Fear,
				_ => throw new ArgumentOutOfRangeException(nameof(emotion))
			};
		}
	}

	public IEnumerable<EmotionalState> GetEmotions() =>
		Enum.GetValues<VillagerEmotion>().Select(e => new EmotionalState(e, this[e]));
}

public enum VillagerEmotion {
	Happiness,
	Sadness,
	Fear
}

public record EmotionalState(VillagerEmotion Emotion, int Intensity);