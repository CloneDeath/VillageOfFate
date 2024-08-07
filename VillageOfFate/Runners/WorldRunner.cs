using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenAi;
using OpenAi.Gpt;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Localization;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.Runners;

public class WorldRunner(
	TimeService time,
	VillagerService villagers,
	VillagerActivityService activities,
	VillagerActionErrorService villagerActionErrors,
	EventsService events,
	ActionFactory actionFactory,
	GptUsageService gptUsage,
	OpenApi openApi,
	RandomProvider random,
	StatusBuilder statusBuilder,
	DataContext context,
	Plurality plurality
) : IRunner {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task RunAsync(CancellationToken cancellationToken) {
		try {
			while (!cancellationToken.IsCancellationRequested) {
				context.ChangeTracker.Clear();

				var worldTime = await GetWorldTimeAsync();
				if (worldTime > await GetEndTimeAsync()) {
					await EnsureVillagersHaveActivities();
					await Task.Delay(Interval, cancellationToken);
					continue;
				}

				var now = DateTime.UtcNow;
				var timeSinceLastUpdate = now - worldTime;

				if (timeSinceLastUpdate >= Interval) {
					await EnsureVillagersHaveActivities();
					await SimulateWorld();
					await SetWorldTimeAsync(worldTime + Interval);
				} else {
					// Sleep until it's time for the next update
					var sleepDuration = Interval - timeSinceLastUpdate;
					await Task.Delay(sleepDuration, cancellationToken);
				}
			}
		}
		catch (OperationCanceledException) {
			Console.WriteLine("WorldRunner was cancelled");
		}
		catch (Exception e) {
			await Console.Error.WriteLineAsync($"WorldRunner threw an exception: {e}");
		}
		finally {
			Console.WriteLine("Exiting WorldRunner");
		}
	}

	public async Task<DateTime> GetWorldTimeAsync() => await time.GetAsync(TimeLabel.World);
	public async Task SetWorldTimeAsync(DateTime value) => await time.SetAsync(TimeLabel.World, value);

	public async Task<DateTime> GetEndTimeAsync() =>
		await time.GetAsync(TimeLabel.End, await GetWorldTimeAsync() + TimeSpan.FromMinutes(2));

	private async Task EnsureVillagersHaveActivities() {
		var idleVillagers = await villagers.GetVillagersWithoutActivities();
		foreach (var idleVillager in idleVillagers) {
			await QueueActionsForVillager(idleVillager);
		}
	}

	private async Task SimulateWorld() {
		foreach (var villager in await villagers.GetAllAsync()) {
			await SimulateVillager(villager);
		}
	}

	private async Task SimulateVillager(VillagerDto villager) {
		var currentTime = await GetWorldTimeAsync();
		var currentActivity = villager.CurrentActivity;
		if (currentActivity == null) {
			if (!villager.ActivityQueue.Any()) {
				await QueueActionsForVillager(villager);
			}

			var nextActivity = villager.ActivityQueue.FirstOrDefault();
			if (nextActivity == null) return;

			var action = actionFactory.Get(nextActivity.Name);
			nextActivity.StartTime = currentTime;
			nextActivity.Status = ActivityStatus.InProgress;
			await activities.SaveAsync(nextActivity);
			var beginResult = await action.Begin(nextActivity);
			await HandleResult(beginResult, new ReactionData {
				Actor = villager,
				Action = nextActivity
			});
			return;
		}

		if (currentActivity.EndTime > currentTime) return;

		var currentAction = actionFactory.Get(currentActivity.Name);
		var endResult = await currentAction.End(currentActivity);
		await activities.CompleteAsync(currentActivity);
		await HandleResult(endResult, new ReactionData {
			Actor = villager,
			Action = currentActivity
		});
	}

	private async Task QueueActionsForVillager(VillagerDto villager, ReactionData? reaction = null) {
		var messages = new List<Message> {
			new() {
				Role = Role.System,
				Content = "You are an NPC in a village. You have a set of actions you can perform. " +
						  "You can choose to perform any number of these actions sequentially (planning ahead), or do nothing (deferring decisions for later). " +
						  "Please always stay in character, and choose actions that make sense for your character, their current mood, situation, recent events they witnessed, and their memories."
			},
			new() {
				Role = Role.User,
				Content = await statusBuilder.BuildVillagerStatusAsync(villager)
			}
		};
		messages.AddRange(villager.WitnessedEvents.Select(e => new Message {
			Role = Role.User,
			Content =
				$"[{e.Time}]@{e.Sector.Position} {e.VillagerActor?.Name ?? e.ItemActor?.Definition.Name ?? "World Event"}: {e.Description}"
		}));
		messages.Add(new Message {
			Role = Role.User,
			Content = "Please choose an action befitting your character."
		});

		var actionChoices = actionFactory.Actions.Select(a => new GptFunction {
			Name = a.Name,
			Description = a.Description,
			Parameters = a.Parameters
		}).ToList();
		const string DoNotReactName = "DoNotReact";
		if (reaction != null) {
			actionChoices.Add(new GptFunction {
				Name = DoNotReactName,
				Description = "Unlike Do Nothing, this action is specifically for when you want to ignore a reaction."
				 + "No time will be wasted on this action, and you can continue with your planned actions."
			});
		}
		var response = await openApi.GetChatGptResponseAsync(messages.ToArray(), actionChoices, ToolChoice.Required);
		await gptUsage.AddUsageAsync(response);

		var calls = response.Choices.First().Message.ToolCalls ?? [];
		var details = new List<ActivityDto>();
		for (var index = 0; index < calls.Length; index++) {
			var call = calls[index];
			if (reaction != null && call.Function.Name == DoNotReactName) {
				await events.AddAsync(villager, $"Decides to ignore {reaction.Actor.Name}'s {reaction.Action.Name.ToActiveString()}");
				continue;
			}
			var action = actionFactory.Get(call.Function.Name);
			if (action == null) {
				await villagerActionErrors.LogInvalidAction(villager, call.Function.Name, call.Function.Arguments);
				continue;
			}

			ActivityDto activity;

			try {
				activity = await action.ParseArguments(call.Function.Arguments);
			}
			catch (Exception e) {
				await villagerActionErrors.LogActionParseError(villager, call.Function.Name, call.Function.Arguments,
					e);
				continue;
			}

			activity.DurationRemaining = activity.TotalDuration;
			activity.Villager = villager;
			activity.Priority = index;
			activity.Status = ActivityStatus.Pending;
			activity.StartTime = null;
			details.Add(activity);
		}

		var reactionVerb = GetReactionVerb(reaction);

		await events.AddAsync(villager,
			$"Decides to {reactionVerb} the following {plurality.Pick(details, "action", "actions")}: {string.Join(", ", details.Select(d => d.Name.ToFutureString()))}");
		foreach (var activityDetail in details) {
			await activities.AddAsync(villager, activityDetail);
		}
	}

	private static string GetReactionVerb(ReactionData? reaction) =>
		reaction == null
			? "perform"
			: $"react to {reaction.Actor.Name}'s {reaction.Action.Name.ToActiveString()} by performing";

	private async Task HandleResult(IActionResults result, ReactionData reaction) {
		if (!result.TriggerReactions.Any()) {
			return;
		}

		var currentTime = await GetWorldTimeAsync();
		var selected = random.SelectOne(result.TriggerReactions);
		var currentActivity = selected.CurrentActivity;
		if (currentActivity != null) {
			if (!currentActivity.EndTime.HasValue) throw new NullReferenceException();
			currentActivity.DurationRemaining = currentActivity.EndTime.Value - currentTime;
			currentActivity.Status = ActivityStatus.OnHold;
			await activities.SaveAsync(currentActivity);
		}

		await QueueActionsForVillager(selected, reaction);
	}
}