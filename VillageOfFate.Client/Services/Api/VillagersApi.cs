using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class VillagersApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<WebVillager[]> GetVillagers() {
		var result = await client.GetFromJsonAsync<WebVillager[]>("/Villagers")
					 ?? throw new Exception("Failed to get Villagers from server.");
		return result;
	}

	public async Task<WebVillager> GetVillagerAsync(Guid id) =>
		await client.GetFromJsonAsync<WebVillager>($"/Villagers/{id}")
		?? throw new Exception($"Failed to get Villager with Id {id} from server.");

	public async Task<WebEvent[]> GetVillagerEvents(Guid id) =>
		await client.GetFromJsonAsync<WebEvent[]>($"/Villagers/{id}/Events")
		?? throw new Exception($"Failed to get Events for Villager with Id {id} from server.");
}