using System;
using System.Reflection;
using System.Threading.Tasks;
using Blazored.SessionStorage;
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

		var clientId = builder.Configuration["GoogleClientId"] ?? throw new Exception("No Google Client Id configured");

		var apiBaseUri = builder.Configuration.GetValue<string>("ApiBaseUri") ??
						 throw new Exception("ApiBaseUri is not set in appsettings.json");

		builder.Services.AddScoped(sp => {
			var access = sp.GetService<ISessionStorageService>() ??
						 throw new NullReferenceException("ISessionStorageService");
			return new ApiClient(apiBaseUri, access, clientId);
		});
		builder.Services.AddBlazoredSessionStorage();
		builder.Services.AddScoped<TimeApi>();
		builder.Services.AddScoped<ItemsApi>();
		builder.Services.AddSingleton(new VillagersApi(apiBaseUri));
		builder.Services.AddSingleton(new ImageApi(apiBaseUri));
		builder.Services.AddScoped<SectorsApi>();
		builder.Services.AddScoped<UserApi>();
		builder.Services.AddScoped<Plurality>();
		builder.Services.AddScoped<NavigationService>();
		RegisterClientServices(builder);

		builder.Services.AddOidcAuthentication(options => {
			options.ProviderOptions.Authority = "https://accounts.google.com";
			options.ProviderOptions.ClientId = clientId;
			options.ProviderOptions.ResponseType = "id_token token";
			options.ProviderOptions.DefaultScopes.Add("openid");
			options.ProviderOptions.DefaultScopes.Add("profile"); // .../auth/userinfo.profile
			options.ProviderOptions.DefaultScopes.Add("email"); // .../auth/userinfo.email
			options.ProviderOptions.RedirectUri = builder.HostEnvironment.BaseAddress + "authentication/login-callback";
			options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
		});

		await builder.Build().RunAsync();
	}

	private static void RegisterClientServices(WebAssemblyHostBuilder builder) {
		var assembly = typeof(RegisterClientServiceAttribute).Assembly;
		foreach (var type in assembly.GetTypes()) {
			if (type.GetCustomAttribute<RegisterClientServiceAttribute>() == null) continue;
			var interfaces = type.GetInterfaces();
			if (interfaces.Length == 0) {
				builder.Services.AddScoped(type);
			} else {
				foreach (var i in interfaces) {
					builder.Services.AddScoped(i, type);
				}
			}
		}
	}
}