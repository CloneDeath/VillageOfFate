using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAi;
using OpenAi.Models;
using VillageOfFate.Actions;
using VillageOfFate.DAL;
using VillageOfFate.Localization;
using VillageOfFate.Runners;
using VillageOfFate.Server.Databases;
using VillageOfFate.Server.Settings;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

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
		builder.Services.AddDbContext<DataContext>(b => {
			dbDetails.BuildContext(b);
			if (appSettings.Database.EnableSensitiveDataLogging) {
				b.EnableSensitiveDataLogging();
			}
		});

		var openApiKey = builder.Configuration["OPENAI_API_KEY"]
						 ?? throw new NullReferenceException("The Secret Configuration 'OPENAI_API_KEY' is not set.");
		builder.Services.AddSingleton(new OpenApi(openApiKey) {
			ChatModel = GptModel.Gpt_4_Omni,
			ImageModel = ImageModel.Dall_E_2
		});

		builder.Services.AddScoped<TimeService>();
		builder.Services.AddScoped<SectorService>();
		builder.Services.AddScoped<VillagerService>();
		builder.Services.AddScoped<VillagerActivityService>();
		builder.Services.AddScoped<VillagerEmotionService>();
		builder.Services.AddScoped<EventsService>();
		builder.Services.AddScoped<VillagerItemService>();
		builder.Services.AddScoped<VillagerActionErrorService>();
		builder.Services.AddScoped<RelationshipService>();
		builder.Services.AddScoped<ItemService>();
		builder.Services.AddScoped<ImageService>();
		builder.Services.AddScoped<GptUsageService>();
		builder.Services.AddSingleton<RandomProvider>();
		builder.Services.AddScoped<WorldInitializer>();
		builder.Services.AddScoped<WorldRunner>();
		builder.Services.AddScoped<ImageGenerationRunner>();
		builder.Services.AddScoped<StatusBuilder>();
		builder.Services.AddScoped<ActionFactory>();

		// Localization
		builder.Services.AddScoped<Plurality>();

		// Actions
		builder.Services.AddScoped<AdjustEmotionalStateAction>();
		builder.Services.AddScoped<EatAction>();
		builder.Services.AddScoped<IdleAction>();
		builder.Services.AddScoped<InteractAction>();
		builder.Services.AddScoped<LookoutAction>();
		builder.Services.AddScoped<SleepAction>();
		builder.Services.AddScoped<SpeakAction>();

		var app = builder.Build();
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions { SourceCodeLineCount = 100 });
		}

		app.UseHttpsRedirection();
		app.UseCors("AllowMyOrigin");
		app.MapDefaultControllerRoute();

		await InitializeWorld(app);

		var cancellationTokenSource = new CancellationTokenSource();
		var appTask = app.RunAsync(cancellationTokenSource.Token);
		var worldRunnerTask = ExecuteRunner<WorldRunner>(app, cancellationTokenSource.Token);
		var imageGenRunnerTask = ExecuteRunner<ImageGenerationRunner>(app, cancellationTokenSource.Token);

		await Task.WhenAny(appTask, worldRunnerTask, imageGenRunnerTask);
		await cancellationTokenSource.CancelAsync();
	}

	private static async Task ExecuteRunner<T>(WebApplication app, CancellationToken token) where T : IRunner {
		await using var scope = app.Services.CreateAsyncScope();
		var runner = scope.ServiceProvider.GetService<T>()
					 ?? throw new NullReferenceException($"Could not get the {typeof(T).Name} runner");
		await runner.RunAsync(token);
	}

	private static async Task InitializeWorld(WebApplication app) {
		await using var scope = app.Services.CreateAsyncScope();
		var initializer = scope.ServiceProvider.GetService<WorldInitializer>()
						  ?? throw new NullReferenceException("Could not get the WorldInitializer");
		await initializer.PopulateWorldAsync();
	}
}