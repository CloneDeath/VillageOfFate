using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenAi;
using OpenAi.Models;
using VillageOfFate.Actions;
using VillageOfFate.DAL;
using VillageOfFate.Localization;
using VillageOfFate.Runners;
using VillageOfFate.Server.Databases;
using VillageOfFate.Server.Exceptions;
using VillageOfFate.Server.Settings;
using VillageOfFate.Services;
using VillageOfFate.Services.BIServices;
using VillageOfFate.Services.DALServices;
using VillageOfFate.Services.DALServices.Core;

namespace VillageOfFate.Server;

public class Program {
	public static async Task Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c => {
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
				Description = """
							  JWT Authorization header using the Bearer scheme.

							  Enter 'Bearer' [space] and then your token in the text input below.

							  Example: 'Bearer 12345abc'
							  """,
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement {
				{
					new OpenApiSecurityScheme {
						Reference = new OpenApiReference {
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header
					},
					new List<string>()
				}
			});
		});
		builder.Services.AddCors(options => {
			options.AddPolicy("AllowMyOrigin",
				p => p
					 .WithOrigins("https://localhost:7036")
					 .AllowAnyHeader()
					 .AllowAnyMethod());
		});

		var clientId = builder.Configuration["GoogleClientId"] ?? throw new Exception("No Google Client Id configured");
		builder.Services.AddAuthentication(options => {
				   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			   })
			   .AddJwtBearer(options => {
				   options.Authority = "https://accounts.google.com";
				   options.TokenValidationParameters = new TokenValidationParameters {
					   ValidateIssuer = true,
					   ValidIssuer = "https://accounts.google.com",
					   ValidateAudience = true,
					   ValidAudience = clientId,
					   ValidateLifetime = true
				   };
				   options.Events = new JwtBearerEvents {
					   OnAuthenticationFailed = context => {
						   Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
						   return Task.CompletedTask;
					   }
				   };
			   });

		builder.Services.AddAuthorization();

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

		builder.Services.AddHttpContextAccessor();
		builder.Services.AddScoped<UserService>();
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
		builder.Services.AddScoped<PlayerInitializer>();
		builder.Services.AddScoped<WorldInitializer>();
		builder.Services.AddScoped<WorldRunner>();
		builder.Services.AddScoped<ImageGenerationRunner>();
		builder.Services.AddScoped<StatusBuilder>();
		builder.Services.AddScoped<ActionFactory>();
		RegisterApiServices(builder);

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
		builder.Services.AddSingleton<ExceptionHandler>();

		var app = builder.Build();
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseDeveloperExceptionPage(new DeveloperExceptionPageOptions { SourceCodeLineCount = 100 });
		}

		app.UseMiddleware<ExceptionHandler>();
		app.UseHttpsRedirection();
		app.UseCors("AllowMyOrigin");
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapDefaultControllerRoute();

		await InitializeWorld(app);

		var cancellationTokenSource = new CancellationTokenSource();
		var tasks = new List<Task> {
			app.RunAsync(cancellationTokenSource.Token),
			ExecuteRunner<WorldRunner>(app, cancellationTokenSource.Token)
		};
		if (appSettings.GenerateImages) {
			tasks.Add(ExecuteRunner<ImageGenerationRunner>(app, cancellationTokenSource.Token));
		}

		await Task.WhenAny(tasks.ToArray());
		await cancellationTokenSource.CancelAsync();
	}

	private static void RegisterApiServices(WebApplicationBuilder builder) {
		var assembly = typeof(RegisterApiServiceAttribute).Assembly;
		foreach (var type in assembly.GetTypes()) {
			if (type.GetCustomAttribute<RegisterApiServiceAttribute>() == null) continue;
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