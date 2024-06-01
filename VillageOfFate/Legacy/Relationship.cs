namespace VillageOfFate.Legacy;

public class Relationship(Villager villager, string relation) {
	public Villager Villager { get; } = villager;
	public string Relation { get; } = relation;
}