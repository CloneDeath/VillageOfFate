namespace VillageOfFate;

public class Villager {
	public string Name { get; set; } = "Villager";
	public int Age { get; set; } = 18;
	public string Summary { get; set; } = string.Empty;
	public Gender Gender { get; set; } = Gender.Male;

	public string GetDescription() {
		return $"{Name} is a {Age} year old {Gender}. Summary: {Summary}";
	}
}

public enum Gender {
	Male,
	Female
}