using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenAi;
using OpenAi.Gpt;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Localization;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;
using VillageOfFate.WebModels;

namespace VillageOfFate.WorldServices;

public class VillagerActionService(
	StatusBuilder statusBuilder,
	ActionFactory actionFactory,
	GptUsageService gptUsage,
	OpenApi openApi,
	EventsService events,
	Plurality plurality,
	VillagerActionErrorService villagerActionErrors,
	VillagerActivityService activities
) {
	public async Task QueueActionsForVillager(VillagerDto villager, ReactionData? reaction = null) {
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
				await events.AddAsync(villager,
					$"Decides to ignore {reaction.Actor?.Name ?? reaction.Item?.Definition.Name}'s {reaction.ActiveActionName}");
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

			activity.Arguments = call.Function.Arguments;
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
			: $"react to {reaction.Actor?.Name ?? reaction.Item?.Definition.Name}'s {reaction.ActiveActionName} by performing";
}