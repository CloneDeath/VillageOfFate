using System;
using System.Collections.Generic;
using System.Drawing;
using VillageOfFate.Activities;

namespace VillageOfFate;

public class Villager {
	private readonly Memory _memory = [];

	private readonly RelationshipMemory _relationship = new();

	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;
	public string Summary { get; set; } = string.Empty;
	public Gender Gender { get; set; } = Gender.Male;
	public VillagerEmotions Emotions { get; set; } = new();
	public Point SectorLocation { get; set; }
	public int Hunger { get; private set; }
	public required Activity CurrentActivity { get; init; }

	public void IncreaseHunger(int amount) => Hunger += amount;
	public void DecreaseHunger(int amount) => Hunger = Math.Max(0, Hunger - amount);

	public void AddMemory(string message) {
		_memory.Add(message);
	}

	public IEnumerable<string> GetMemory() => _memory;

	public void AddRelationship(Villager villager, string relationship) {
		_relationship.Add(villager, relationship);
	}

	public IEnumerable<Relationship> GetRelationships() => _relationship;

	public string GetDescription() => $"{Name} is a {Age} year old {Gender}. Summary: {Summary}";

	public void AdjustEmotion(VillagerEmotion emotion, int adjustment) => Emotions.AdjustEmotion(emotion, adjustment);

	public IEnumerable<EmotionalState> GetEmotions() => Emotions.GetEmotions();
}

public enum Gender {
	Male,
	Female
}