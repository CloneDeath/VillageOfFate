using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class SectorsApi(ApiClient client) {
	public async Task<IReadOnlyList<WebSector>> GetAllAsync() =>
		await client.GetAsync<IReadOnlyList<WebSector>>("/Sectors");

	public async Task<WebSector> GetSectorAsync(Guid sectorId) =>
		await client.GetAsync<WebSector>($"/Sectors/{sectorId}");

	public async Task<int> GetVillagerCountAsync(Guid sectorId) =>
		await client.GetAsync<int>($"/Sectors/{sectorId}/VillagerCount");

	public async Task<IEnumerable<WebVillager>> GetVillagersAsync(Guid sectorId) =>
		await client.GetAsync<IEnumerable<WebVillager>>($"/Sectors/{sectorId}/Villagers");

	public async Task<IEnumerable<WebEvent>?> GetEventsAsync(Guid sectorId) =>
		await client.GetAsync<IEnumerable<WebEvent>>($"/Sectors/{sectorId}/Events");
}