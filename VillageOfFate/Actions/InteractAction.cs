using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VillageOfFate.Actions.Parameters;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Actions;

public class InteractAction(EventsService events, VillagerService villagers) : IAction {
	public string Name => "Interact";
	public ActivityName ActivityName => ActivityName.Interact;

	public string Description => "Interact with someone else";
	public object Parameters => ParameterBuilder.GenerateJsonSchema<InteractArguments>();

	public async Task<ActivityDto> ParseArguments(string arguments) {
		var args = JsonSerializer.Deserialize<InteractArguments>(arguments)
				   ?? throw new NullReferenceException();
		var targets = await villagers.GetManyAsync(args.VillagerIds);
		return new InteractActivityDto {
			Description = "Interacting",
			Duration = TimeSpan.FromSeconds(args.DurationInSeconds),
			Interruptible = true,
			Action = args.Action,
			Targets = targets.ToArray()
		};
	}

	public async Task<IActionResults> Begin(ActivityDto activityDto) {
		if (activityDto is not InteractActivityDto interactActivity) {
			throw new ArgumentException("ActivityDto is not an InteractActivityDto");
		}

		var villager = interactActivity.Villager;
		var targets = await villagers.GetManyAsync(interactActivity.Targets);
		var targetNames = joinNames(targets.Select(t => t.Name).ToList());
		var actionDescription = interactActivity.Description;
		if (!actionDescription.Contains("{target}") && !actionDescription.Contains("{targets}")) {
			throw new ArgumentException("Action description must contain either {target} or {targets}");
		}

		actionDescription = actionDescription.Replace("{target}", targetNames);
		actionDescription = actionDescription.Replace("{targets}", targetNames);

		var description = $"{villager.Name} {actionDescription}";
		await events.AddAsync(villager, villager.Sector.Villagers, description);
		return new ActionResults();
	}

	public Task<IActionResults> End(ActivityDto activityDto) {
		if (activityDto is not InteractActivityDto interactActivity) {
			throw new ArgumentException("ActivityDto is not an InteractActivityDto");
		}

		return Task.FromResult<IActionResults>(new ActionResults {
			TriggerReactions = interactActivity.Targets.ToList()
		});
	}

	private static string joinNames(IReadOnlyList<string> names) {
		switch (names.Count) {
			case 0: return "No one";
			case 1: return names[0];
		}

		var last = names[^1];
		var others = names.SkipLast(1);
		return $"{string.Join(", ", others)}, and {last}";
	}
}

public class InteractArguments {
	[JsonRequired]
	[JsonPropertyName("villagerIds")]
	[JsonDescription("the villager id(s) the action is directed at")]
	public Guid[] VillagerIds { get; set; } = [];

	[JsonRequired]
	[JsonPropertyName("durationInSeconds")]
	[JsonDescription("The number of seconds the interaction takes")]
	public double DurationInSeconds { get; set; }

	[JsonRequired]
	[JsonPropertyName("action")]
	[JsonDescription("A description of the interaction you are performing." +
					 " You MUST use {target} or {targets} to refer to the villager(s) you are interacting with." +
					 " Also, the description will automatically be prepended with your name, ie \"holds {target}'s hand.\" will become \"Marry holds John's hand.\"." +
					 " DO NOT include your name at the start of the action, it will be added automatically." +
					 " Make sure to pay close attention to everyone's gender, and to use the correct pronouns, especially your own." +
					 " Some examples are:" +
					 "\n \"scares {target}.\" becomes \"Michael scares Janet.\"" +
					 "\n \"comforts the crying children {targets}.\" becomes \"Roe comforts the crying children Penny and Sue\"" +
					 "\n \"offers some food to {targets}.\" becomes \"Mark offers some food to Richard, Blake, and Amy\"")]
	public string Action { get; set; } = string.Empty;
}