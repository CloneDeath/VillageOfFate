using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace VillageOfFate.Client.Services.Api;

public class TimeApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<DateTime> GetTime() {
		var result = await client.GetFromJsonAsync<string>("/Time")
					 ?? throw new Exception("Failed to get time from server.");
		return DateTime.Parse(result);
	}
}