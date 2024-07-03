using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace VillageOfFate.Client.Services;

public class ApiClient(string baseUrl, IAccessTokenProvider accessToken) {
	private HttpClient client { get; } = new() {
		BaseAddress = new Uri(baseUrl)
	};

	private async Task<AuthenticationHeaderValue?> GetAuthenticationHeader() {
		var request = await accessToken.RequestAccessToken();
		Console.WriteLine($"Token request status: {request.Status}");
		var gotToken = request.TryGetToken(out var token);
		Console.WriteLine($"{gotToken} {token.Value}");
		return gotToken
				   ? new AuthenticationHeaderValue("Bearer", token.Value)
				   : null;
	}

	public async Task<T> GetAsync<T>(string requestUri) {
		client.DefaultRequestHeaders.Authorization = await GetAuthenticationHeader();
		return await client.GetFromJsonAsync<T>(requestUri)
			   ?? throw new Exception($"Failed to get {typeof(T).Name} from {requestUri}.");
	}

	public async Task PostAsync<T>(string requestUri, T body) {
		client.DefaultRequestHeaders.Authorization = await GetAuthenticationHeader();
		await client.PostAsJsonAsync(requestUri, body);
	}
}