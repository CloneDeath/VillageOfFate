using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class IdleAction(
	EventsService eventService
) : IAction {
	public string Name => "DoNothing";
	public ActivityName ActivityName => ActivityName.Idle;
	public string Description => "You will do nothing, sit idle, and wait for something to happen.";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<IdleArguments>();

	public Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<IdleArguments>(arguments)
				   ?? throw new NullReferenceException();
		return Task.FromResult<ActivityDto>(new IdleActivityDto {
			TotalDuration = TimeSpan.FromHours(args.DurationInHours)
		});
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		await eventService.AddAsync(activityDto.Villager, $"{activityDto.Villager.Name} takes a break.");
		return new ActionResults();
	}

	public Task<IActionResults> End(ActivityDto activityDto) => Task.FromResult<IActionResults>(new ActionResults());
}

public class IdleArguments {
	[JsonRequired]
	[JsonPropertyName("durationInHours")]
	[JsonDescription("how long to stay idle")]
	public double DurationInHours { get; set; }
}