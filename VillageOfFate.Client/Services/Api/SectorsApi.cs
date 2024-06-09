using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class SectorsApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<IReadOnlyList<WebSector>> GetAllAsync() =>
		await client.GetFromJsonAsync<IReadOnlyList<WebSector>>("/Sectors")
		?? throw new Exception("Failed to get Sectors from server.");

	public async Task<WebSector> GetSectorAsync(Guid id) =>
		await client.GetFromJsonAsync<WebSector>($"/Sectors/{id}")
		?? throw new Exception("Failed to get Sector from server.");
}