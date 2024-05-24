using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VillageOfFate.Client.Services.Api;

namespace VillageOfFate.Client;

public class Program {
	public static async Task Main(string[] args) {
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		var apiBaseUri = builder.Configuration.GetValue<string>("ApiBaseUri") ??
						 throw new Exception("ApiBaseUri is not set in appsettings.json");
		builder.Services.AddSingleton(new TimeApi(apiBaseUri));
		builder.Services.AddSingleton(new VillagersApi(apiBaseUri));

		builder.Services.AddOidcAuthentication(options => {
			// Configure your authentication provider options here.
			// For more information, see https://aka.ms/blazor-standalone-auth
			builder.Configuration.Bind("Local", options.ProviderOptions);
		});

		await builder.Build().RunAsync();
	}
}