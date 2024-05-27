using System;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Migrations.Runner;

public abstract class BaseMigrationRunner {
	public virtual void ExecuteMigration() {
		var serviceProvider = CreateService();

		using var scope = serviceProvider.CreateScope();
		UpdateDatabase(scope.ServiceProvider);
	}

	private static void UpdateDatabase(IServiceProvider serviceProvider) {
		var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
		runner.MigrateUp();
	}

	protected virtual IServiceProvider CreateService() {
		return new ServiceCollection()
			   .AddFluentMigratorCore()
			   .ConfigureRunner(
				   rb => SetupMigrationRunnerBuilder(rb.ScanIn(typeof(InitialCreate).Assembly).For.Migrations())
			   )
			   .AddLogging(lb => lb.AddFluentMigratorConsole())
			   .BuildServiceProvider(false);
	}

	protected abstract void SetupMigrationRunnerBuilder(IMigrationRunnerBuilder rb);
}