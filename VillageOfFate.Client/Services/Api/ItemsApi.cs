using System;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class ItemsApi(ApiClient client) {
	public async Task<WebItem> GetItemAsync(Guid id) => await client.GetAsync<WebItem>($"/Items/{id}");

	public async Task<WebItemLocation> GetItemLocationAsync(Guid id) =>
		await client.GetAsync<WebItemLocation>($"Items/{id}/Location");
}