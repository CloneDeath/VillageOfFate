using System;
using System.Threading.Tasks;
using GptApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VillageOfFate.DAL;
using VillageOfFate.Server.Databases;
using VillageOfFate.Server.Settings;
using VillageOfFate.Services.DALServices;

namespace VillageOfFate.Server;

public class Program {
	public static async Task Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddCors(options => {
			options.AddPolicy("AllowMyOrigin",
				p => p
					 .WithOrigins("https://localhost:7036")
					 .AllowAnyHeader()
					 .AllowAnyMethod());
		});

		var appSettingsSection = builder.Configuration.GetSection("AppSettings");
		builder.Services.Configure<AppSettings>(appSettingsSection);
		var appSettings = appSettingsSection.Get<AppSettings>()
						  ?? throw new NullReferenceException("AppSettings object is null");
		var dbDetails = DatabaseFactory.GetHandlerFor(appSettings.Database);
		dbDetails.RunMigration();
		builder.Services.AddDbContext<DataContext>(dbDetails.BuildContext);

		builder.Services.AddSingleton(GenerateWorld());
		builder.Services.AddScoped<TimeService>();
		builder.Services.AddScoped<SectorService>();
		builder.Services.AddScoped<VillagerService>();
		builder.Services.AddScoped<ItemService>();
		builder.Services.AddSingleton<RandomProvider>();
		builder.Services.AddSingleton(CreateChatGptApiService());
		builder.Services.AddSingleton<WorldInitializer>();

		var app = builder.Build();
		var initializer = app.Services.GetService<WorldInitializer>()
						  ?? throw new NullReferenceException("Could not get the WorldInitializer");
		await initializer.PopulateWorldAsync();

		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();
		app.UseCors("AllowMyOrigin");
		app.MapDefaultControllerRoute();
		await app.RunAsync();
	}

	private static ChatGptApi CreateChatGptApiService() {
		var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
					 ?? throw new InvalidOperationException("The environment variable 'OPENAI_API_KEY' is not set.");
		return new ChatGptApi(apiKey) {
			Model = GptModel.Gpt_4_Omni
		};
	}

	private static World GenerateWorld() {
		var random = new RandomProvider();
		var world = VillageOfFate.Program.GetInitialWorld();
		VillageOfFate.Program.GetInitialVillagers(world, random);
		return world;
	}
}