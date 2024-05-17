using System;
using VillageOfFate.Activities;

namespace VillageOfFate.VillagerActions;

public class DoNothingAction : IVillagerAction {
	public string Name => "DoNothing";
	public string Description => "Do nothing, and observe";
	public object? Parameters => null;

	public IActivityDetails Execute(string arguments, VillagerActionState state) =>
		new ActivityDetails {
			Description = "Doing Nothing",
			Duration = TimeSpan.FromSeconds(10)
		};
}