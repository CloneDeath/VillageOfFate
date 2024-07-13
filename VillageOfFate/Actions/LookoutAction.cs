using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class LookoutAction(EventsService events) : IAction {
	public string Name => "Lookout";
	public ActivityName ActivityName => ActivityName.Lookout;

	public string Description => "Keep a Lookout for monsters";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<LookoutArguments>();

	public Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<LookoutArguments>(arguments)
				   ?? throw new NullReferenceException();
		return Task.FromResult<ActivityDto>(new LookoutActivityDto {
			TotalDuration = TimeSpan.FromHours(args.DurationInHours)
		});
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		if (activityDto is not LookoutActivityDto lookoutActivity) {
			throw new ArgumentException("ActivityDto is not a LookoutActivityDto");
		}

		var villager = lookoutActivity.Villager;
		await events.AddAsync(villager, villager.Sector.Villagers, $"{villager.Name} starts to lookout for monsters");
		return new ActionResults();
	}

	public async Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not LookoutActivityDto lookoutActivity) {
			throw new ArgumentException("ActivityDto is not a LookoutActivityDto");
		}

		var villager = lookoutActivity.Villager;
		var completionActivity = $"{villager.Name} finishes their lookout duty";
		await events.AddAsync(villager, villager.Sector.Villagers, completionActivity);

		return new ActionResults();
	}
}

public class LookoutArguments {
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to keep a lookout in hours")]
	public double DurationInHours { get; set; }
}