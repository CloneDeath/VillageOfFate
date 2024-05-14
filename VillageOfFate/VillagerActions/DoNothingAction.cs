namespace VillageOfFate.VillagerActions;

public class DoNothingAction : IVillagerAction {
	public string Name => "DoNothing";
	public string Description => "Do nothing, and observe";
	public object? Parameters => null;
}