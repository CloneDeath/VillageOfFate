using System;
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
	public static void Main(string[] args) {
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

		var world = GenerateWorld();
		builder.Services.AddSingleton(world);
		builder.Services.AddScoped<TimeService>();

		var app = builder.Build();

		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();
		app.UseCors("AllowMyOrigin");
		app.MapDefaultControllerRoute();
		app.Run();
	}

	private static World GenerateWorld() {
		var random = new RandomProvider();
		var world = VillageOfFate.Program.GetInitialWorld();
		VillageOfFate.Program.GetInitialVillagers(world, random);
		return world;
	}
}