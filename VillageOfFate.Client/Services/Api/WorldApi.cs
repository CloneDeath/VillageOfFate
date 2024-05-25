using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class WorldApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<WebWorld> GetWorld() {
		var result = await client.GetFromJsonAsync<WebWorld>("/World")
					 ?? throw new Exception("Failed to get World from server.");
		return result;
	}
}