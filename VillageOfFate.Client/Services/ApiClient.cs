using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace VillageOfFate.Client.Services;

public class ApiClient(HttpClient client) {
	public async Task<T> GetAsync<T>(string requestUri) =>
		await client.GetFromJsonAsync<T>(requestUri)
		?? throw new Exception($"Failed to get {typeof(T).Name} from {requestUri}.");

	public async Task PostAsync<T>(string requestUri, T body) => await client.PostAsJsonAsync(requestUri, body);
}