using System;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class ItemsApi(ApiClient client) {
	public async Task<WebItem> GetItemAsync(Guid id) => await client.GetAsync<WebItem>($"/items/{id}");

	public async Task<WebItemLocation> GetItemLocationAsync(Guid itemId) =>
		await client.GetAsync<WebItemLocation>($"items/{itemId}/location");

	public async Task<WebItem[]?> GetPagesInItem(Guid itemId) =>
		await client.GetAsync<WebItem[]>($"items/{itemId}/pages");
}