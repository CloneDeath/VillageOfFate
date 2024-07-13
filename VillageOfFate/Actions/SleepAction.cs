using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class SleepAction(EventsService events) : IAction {
	public string Name => "Sleep";
	public ActivityName ActivityName => ActivityName.Sleep;

	public string Description => "Sleep for a while";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<SleepArguments>();

	public Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<SleepArguments>(arguments)
				   ?? throw new NullReferenceException();
		return Task.FromResult<ActivityDto>(new SleepActivityDto {
			TotalDuration = TimeSpan.FromHours(args.DurationInHours)
		});
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		if (activityDto is not SleepActivityDto sleepActivity) {
			throw new ArgumentException("ActivityDto is not a SleepActivityDto");
		}

		var villager = sleepActivity.Villager;
		var description = $"{villager.Name} lays down to rest";
		await events.AddAsync(villager, villager.Sector.Villagers, description);
		return new ActionResults();
	}

	public async Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not SleepActivityDto sleepActivity) {
			throw new ArgumentException("ActivityDto is not a SleepActivityDto");
		}

		var villager = activityDto.Villager;
		var description = $"{villager.Name} wakes up from an {sleepActivity.TotalDuration.Hours}-hour rest.";
		await events.AddAsync(villager, villager.Sector.Villagers, description);
		return new ActionResults();
	}
}

public class SleepArguments {
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to sleep in hours")]
	public double DurationInHours { get; set; }
}