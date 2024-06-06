using VillageOfFate.DAL.Entities;

namespace VillageOfFate;

public interface IAction {
	string Name { get; }
	string Description { get; }
	object? Parameters { get; }

	ActivityDto ParseArguments(string arguments);
	IActionResults Begin(ActivityDto activityDto);
	IActionResults End(ActivityDto activityDto);
}