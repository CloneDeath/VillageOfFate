using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VillageOfFate.Client.Services.Api;

public class TimeApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<DateTime> GetTime() {
		var result = await client.GetStringAsync("/Time");
		return DateTime.Parse(result);
	}
}