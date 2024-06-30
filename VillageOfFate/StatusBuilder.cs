using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageOfFate.DAL.Entities.Villagers;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate;

public class StatusBuilder(RelationshipService relationships) {
	public async Task<string> BuildVillagerStatusAsync(VillagerDto villager) {
		var results = new List<string> {
			$"Respond as {villager.Name} would. {villager.GetDescription()}",
			"Keep your gender, age, role, history, and personality in mind.",
			"Act like a real person in a fantasy world. Don't declare your actions, just do them."
		};
		await foreach (var entry in GetRelationships(villager)) {
			results.Add(entry);
		}

		results.AddRange(GetEmotions(villager));
		results.AddRange(GetLocation(villager));
		results.AddRange(GetStatus(villager));
		results.AddRange(GetInventory(villager));
		return string.Join(Environment.NewLine, results);
	}

	private async IAsyncEnumerable<string> GetRelationships(VillagerDto villager) {
		var relations = await relationships.GetAsync(villager);
		yield return "### Relationships";
		foreach (var r in relations) {
			yield return $"- {r.Relation.Name} (${r.RelationId}): {r.Relation.GetDescription()} Relation: {r.Relation}";
		}
	}

	private static IEnumerable<string> GetEmotions(VillagerDto villager) {
		yield return "### Emotions (0% = neutral, 100% = maximum intensity)";
		foreach (var e in villager.Emotions.GetEmotions()) {
			yield return $"- {e.Emotion}: {e.Intensity}%";
		}
	}

	private static IEnumerable<string> GetLocation(VillagerDto villager) {
		yield return "### Location";
		yield return $"You are located at Sector Coordinate {villager.Sector.Position}.";
		yield return $"Description: {villager.Sector.Description}";
		yield return "Location Items:";
		foreach (var item in villager.Sector.Items) {
			yield return $"- {item.GetSummary()}";
		}
	}

	private static IEnumerable<string> GetStatus(VillagerDto villager) {
		yield return "### Status";
		yield return $"- Hunger: {villager.Hunger} (+1 per hour)";
	}

	private static IEnumerable<string> GetInventory(VillagerDto villager) {
		yield return "### Inventory";
		foreach (var item in villager.Items) {
			yield return $"- {item.GetSummary()}";
		}
	}
}