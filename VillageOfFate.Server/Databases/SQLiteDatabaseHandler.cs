using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Migrations.Runner;
using VillageOfFate.Server.Settings;

namespace VillageOfFate.Server.Databases;

// ReSharper disable once InconsistentNaming
public class SQLiteDatabaseHandler(DatabaseDetails databaseDetails) : IDatabaseHandler {
	public void BuildContext(DbContextOptionsBuilder builder) {
		SQLiteDataContext.ConfigureOptionsBuilder(builder, databaseDetails.DataSource);
	}

	public void RunMigration() {
		var sqLiteRunner = new SQLiteMigrationRunner(databaseDetails.DataSource);
		sqLiteRunner.ExecuteMigration();
	}
}