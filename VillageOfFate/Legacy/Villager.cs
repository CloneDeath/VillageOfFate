using System;
using System.Collections.Generic;
using VillageOfFate.Legacy.Activities;
using VillageOfFate.WebModels;

namespace VillageOfFate.Legacy;

public class Villager {
	private readonly Memory _memory = [];

	public string Name { get; set; } = "Villager";
	public VillagerEmotions Emotions { get; set; } = new();
	public Position SectorLocation { get; set; }
	public int Hunger { get; private set; }
	public required Activity CurrentActivity { get; set; }
	public Stack<Activity> ActivityQueue { get; } = new();
	public void DecreaseHunger(int amount) => Hunger = Math.Max(0, Hunger - amount);

	public void AddMemory(string message) {
		_memory.Add(message);
	}

	public void AdjustEmotion(VillagerEmotion emotion, int adjustment) => Emotions.AdjustEmotion(emotion, adjustment);
}