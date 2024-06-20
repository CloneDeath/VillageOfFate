using System.Threading.Tasks;
using VillageOfFate.DAL.Entities;
using VillageOfFate.WebModels;

namespace VillageOfFate;

public interface IAction {
	string Name { get; }
	ActivityName ActivityName { get; }
	string Description { get; }
	object? Parameters { get; }

	ActivityDto ParseArguments(string arguments);
	Task<IActionResults> Begin(ActivityDto activityDto);
	Task<IActionResults> End(ActivityDto activityDto);
}