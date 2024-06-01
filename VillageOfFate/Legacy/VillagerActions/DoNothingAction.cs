using System;
using VillageOfFate.Legacy.Activities;

namespace VillageOfFate.Legacy.VillagerActions;

public class DoNothingAction : IVillagerAction {
	public string Name => "DoNothing";
	public string Description => "Do nothing, and observe";
	public object? Parameters => null;

	public IActivityDetails Execute(string arguments, VillagerActionState state) =>
		new ActivityDetails {
			Description = "Doing Nothing",
			Interruptible = true,
			Duration = TimeSpan.FromSeconds(10)
		};
}