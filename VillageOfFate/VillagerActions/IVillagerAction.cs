namespace VillageOfFate.VillagerActions;

public interface IVillagerAction {
	public string Name { get; }
	public string Description { get; }
	public object? Parameters { get; }
}