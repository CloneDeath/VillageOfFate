using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace VillageOfFate.Client.Services.Api;

public class ImageApi(string baseUrl) {
	private readonly HttpClient client = new() {
		BaseAddress = new Uri(baseUrl)
	};

	public async Task<string> GetImageUrlAsync(Guid imageId) {
		var response = await client.GetAsync($"Images/{imageId}");
		return response.StatusCode == HttpStatusCode.Accepted
				   ? "/images/placeholder.png"
				   : $"{baseUrl}/Images/{imageId}";
	}
}