using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blazored.SessionStorage;

namespace VillageOfFate.Client.Services;

public class ApiClient(string baseUrl, ISessionStorageService session, string googleClientId) {
	private HttpClient client { get; } = new() {
		BaseAddress = new Uri(baseUrl)
	};

	private async Task<AuthenticationHeaderValue?> GetAuthenticationHeader() {
		var oidcInfo = await session.GetItemAsync<OidcUser>($"oidc.user:https://accounts.google.com:{googleClientId}");
		if (oidcInfo == null) {
			Console.WriteLine("Token Missing");
			return null;
		}

		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		var expires = epoch.AddSeconds(oidcInfo.ExpiresAt);
		if (expires < DateTime.UtcNow) {
			Console.WriteLine("Token Expired");
			return null;
		}

		return new AuthenticationHeaderValue("Bearer", oidcInfo.IdToken);
		// var state = await auth.GetAuthenticationStateAsync();
		// var user = state.User;
		// foreach (var claim in user.Claims) {
		// 	Console.WriteLine($"Claim: {claim.Type} {claim.Value}");
		// }
		//
		// foreach (var id in user.Identities) {
		// 	Console.WriteLine($"ID: {id.Name} {id.AuthenticationType} {id.IsAuthenticated} {id.Claims.Count()} {id.BootstrapContext}");
		// }
		// var idToken = user.FindFirst(c => c.Type == "id_token")?.Value;
		// var header = idToken != null
		// 				 ? new AuthenticationHeaderValue("Bearer", idToken)
		// 				 : null;
		//Console.WriteLine($"Bearer {idToken ?? "null"}");

		// var result = await token.RequestAccessToken(new AccessTokenRequestOptions {
		// 	Scopes = new[] { "openid", "profile", "email" }
		// });
		// var header = result.TryGetToken(out var idToken)
		// 				 ? new AuthenticationHeaderValue("Bearer", idToken.Value)
		// 				 : null;
		// Console.WriteLine($"Bearer {idToken?.Value ?? null}");
		// return header;
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

public class OidcUser {
	[JsonPropertyName("id_token")] public required string IdToken { get; set; }
	[JsonPropertyName("access_token")] public required string AccessToken { get; set; }
	[JsonPropertyName("token_type")] public required string TokenType { get; set; }
	[JsonPropertyName("scope")] public required string Scope { get; set; }
	[JsonPropertyName("profile")] public required OidcUserProfile Profile { get; set; }
	[JsonPropertyName("expires_at")] public required long ExpiresAt { get; set; }
}

public class OidcUserProfile {
	[JsonPropertyName("azp")] public required string azp { get; set; }
	[JsonPropertyName("sub")] public required string sub { get; set; }
	[JsonPropertyName("email")] public required string email { get; set; }
	[JsonPropertyName("email_verified")] public required bool email_verified { get; set; }
	[JsonPropertyName("name")] public required string name { get; set; }
	[JsonPropertyName("picture")] public required string picture { get; set; }
	[JsonPropertyName("given_name")] public required string given_name { get; set; }
	[JsonPropertyName("family_name")] public required string family_name { get; set; }
	[JsonPropertyName("jti")] public required string jti { get; set; }
}