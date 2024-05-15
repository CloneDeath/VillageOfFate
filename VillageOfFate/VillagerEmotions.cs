using System;
using System.Collections.Generic;
using System.Linq;

namespace VillageOfFate;

public class VillagerEmotions {
	public int Happiness { get; set; }
	public int Sadness { get; set; }
	public int Fear { get; set; }

	public int this[VillagerEmotion emotion] {
		get {
			return emotion switch {
				VillagerEmotion.Happiness => Happiness,
				VillagerEmotion.Sadness => Sadness,
				VillagerEmotion.Fear => Fear,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
		set {
			switch (emotion) {
				case VillagerEmotion.Happiness:
					Happiness = value;
					break;
				case VillagerEmotion.Sadness:
					Sadness = value;
					break;
				case VillagerEmotion.Fear:
					Fear = value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public void AdjustEmotion(VillagerEmotion emotion, int adjustment) {
		var current = this[emotion];
		var newValue = Math.Clamp(current + adjustment, 0, 100);
		this[emotion] = newValue;
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