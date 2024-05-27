using FluentMigrator.Runner;

namespace VillageOfFate.DAL.Migrations.Runner;

// ReSharper disable once InconsistentNaming
public class SQLiteMigrationRunner(string databaseFilepath) : BaseMigrationRunner {
	protected override void SetupMigrationRunnerBuilder(IMigrationRunnerBuilder rb) {
		rb.AddSQLite().WithGlobalConnectionString($"Data Source={databaseFilepath}");
	}
}