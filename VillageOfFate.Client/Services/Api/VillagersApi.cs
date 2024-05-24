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

	public async Task<WebVillager> GetVillager(Guid id) {
		return await client.GetFromJsonAsync<WebVillager>($"/Villagers/{id}")
			   ?? throw new Exception($"Failed to get Villager with Id {id} from server.");
	}
}