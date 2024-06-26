using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillageOfFate.Client.Services;
using VillageOfFate.Client.Services.Api;
using VillageOfFate.Localization;

namespace VillageOfFate.Client;

public class Program {
	public static async Task Main(string[] args) {
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		var apiBaseUri = builder.Configuration.GetValue<string>("ApiBaseUri") ??
						 throw new Exception("ApiBaseUri is not set in appsettings.json");
		builder.Services.AddSingleton(new ApiClient(apiBaseUri));
		builder.Services.AddSingleton<TimeApi>();
		builder.Services.AddSingleton(new ItemsApi(apiBaseUri));
		builder.Services.AddSingleton(new VillagersApi(apiBaseUri));
		builder.Services.AddSingleton(new ImageApi(apiBaseUri));
		builder.Services.AddSingleton<SectorsApi>();
		builder.Services.AddSingleton<Plurality>();
		builder.Services.AddSingleton<NavigationService>();

		var clientId = builder.Configuration["GoogleClientId"] ?? throw new Exception("No Google Client Id configured");

		builder.Services.AddOidcAuthentication(options => {
			options.ProviderOptions.Authority = "https://accounts.google.com";
			options.ProviderOptions.ClientId = clientId;
			options.ProviderOptions.DefaultScopes.Add("openid");
			options.ProviderOptions.DefaultScopes.Add("profile"); // .../auth/userinfo.profile
			options.ProviderOptions.DefaultScopes.Add("email"); // .../auth/userinfo.email
			options.ProviderOptions.RedirectUri = builder.HostEnvironment.BaseAddress + "authentication/login-callback";
			options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
		});

		await builder.Build().RunAsync();
	}
}