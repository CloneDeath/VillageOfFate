using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services.Api;

public class SectorApi(string baseUri) {
	private readonly HttpClient client = new() { BaseAddress = new Uri(baseUri) };

	public async Task<WebSector> GetSectorAsync(Guid id) =>
		await client.GetFromJsonAsync<WebSector>($"/Sector/{id}")
		?? throw new Exception("Failed to get Sector from server.");
}