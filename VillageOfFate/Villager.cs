using System.Collections.Generic;

namespace VillageOfFate;

public class Villager {
	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;
	public string Summary { get; set; } = string.Empty;
	public Gender Gender { get; set; } = Gender.Male;

	private readonly Memory _memory = [];
	public void AddMemory(string message) {
		_memory.Add(message);
	}
	public IEnumerable<string> GetMemory() => _memory;

	private readonly RelationshipMemory _relationship = new();
	public void AddRelationship(Villager villager, string relationship) {
		_relationship.Add(villager, relationship);
	}
	public IEnumerable<Relationship> GetRelationships() => _relationship;

	public string GetDescription() {
		return $"{Name} is a {Age} year old {Gender}. Summary: {Summary}";
	}
}

public enum Gender {
	Male,
	Female
}