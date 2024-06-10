using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class SectorsApi(ApiClient client) {
	public async Task<IReadOnlyList<WebSector>> GetAllAsync() =>
		await client.GetAsync<IReadOnlyList<WebSector>>("/Sectors");

	public async Task<WebSector> GetSectorAsync(Guid id) => await client.GetAsync<WebSector>($"/Sectors/{id}");

	public async Task<int> GetVillagerCountAsync(Guid id) => await client.GetAsync<int>($"/Sectors/{id}/VillagerCount");
}