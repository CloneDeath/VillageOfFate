using System;
using System.Threading.Tasks;

namespace VillageOfFate.Client.Services.Api;

public class TimeApi(ApiClient client) {
	public async Task<DateTime> GetWorldTime() {
		var result = await client.GetAsync<string>("/Time/World");
		return DateTime.Parse(result);
	}

	public async Task<DateTime> GetEndTime() {
		var result = await client.GetAsync<string>("/Time/End");
		return DateTime.Parse(result);
	}

	public async Task AddTime(TimeSpan time) {
		await client.PostAsync("/Time/Add", time);
	}
}