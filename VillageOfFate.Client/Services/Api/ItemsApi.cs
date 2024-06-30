using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class ItemsApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<WebItem> GetItemAsync(Guid id) =>
		await client.GetFromJsonAsync<WebItem>($"/Items/{id}")
		?? throw new Exception($"Failed to get Item with Id {id} from server.");
}