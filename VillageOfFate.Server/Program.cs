using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using GptApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VillageOfFate.DAL;
using VillageOfFate.Legacy;
using VillageOfFate.Server.Databases;
using VillageOfFate.Server.Settings;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Server;

public class Program {
	public static async Task Main(string[] args) {
		var parser = new Parser(with => with.HelpWriter = Console.Error);
		var result = parser.ParseArguments<ProgramOptions>(args);

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
		builder.Services.AddDbContext<DataContext>(b => {
			dbDetails.BuildContext(b);
			if (appSettings.Database.EnableSensitiveDataLogging) {
				b.EnableSensitiveDataLogging();
			}
		});

		var openApiKey = builder.Configuration["OPENAI_API_KEY"]
						 ?? throw new NullReferenceException("The Secret Configuration 'OPENAI_API_KEY' is not set.");
		builder.Services.AddSingleton(new ChatGptApi(openApiKey) {
			Model = GptModel.Gpt_4_Omni
		});

		builder.Services.AddSingleton(new VillageLogger(result.Value.LogDirectory ?? Directory.GetCurrentDirectory()));
		builder.Services.AddScoped<TimeService>();
		builder.Services.AddScoped<SectorService>();
		builder.Services.AddScoped<VillagerService>();
		builder.Services.AddScoped<VillagerActivityService>();
		builder.Services.AddScoped<VillagerMemoryService>();
		builder.Services.AddScoped<VillagerItemService>();
		builder.Services.AddScoped<RelationshipService>();
		builder.Services.AddScoped<ItemService>();
		builder.Services.AddSingleton<RandomProvider>();
		builder.Services.AddScoped<WorldInitializer>();
		builder.Services.AddScoped<WorldRunner>();
		builder.Services.AddSingleton<ActivityFactory>();

		var app = builder.Build();
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions { SourceCodeLineCount = 100 });
		}

		app.UseHttpsRedirection();
		app.UseCors("AllowMyOrigin");
		app.MapDefaultControllerRoute();

		await using var scope = app.Services.CreateAsyncScope();
		var initializer = scope.ServiceProvider.GetService<WorldInitializer>()
						  ?? throw new NullReferenceException("Could not get the WorldInitializer");
		await initializer.PopulateWorldAsync();

		var runner = scope.ServiceProvider.GetService<WorldRunner>()
					 ?? throw new NullReferenceException("Could not get the WorldRunner");

		var cancellationTokenSource = new CancellationTokenSource();
		var appTask = app.RunAsync(cancellationTokenSource.Token);
		var runnerTask = runner.RunAsync(cancellationTokenSource.Token);

		await Task.WhenAny(appTask, runnerTask);
		await cancellationTokenSource.CancelAsync();
	}
}