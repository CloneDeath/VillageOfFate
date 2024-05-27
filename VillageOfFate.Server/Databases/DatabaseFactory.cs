using System;
using VillageOfFate.Server.Settings;

namespace VillageOfFate.Server.Databases;

public static class DatabaseFactory {
	public static IDatabaseHandler GetHandlerFor(DatabaseDetails databaseDetails) {
		return databaseDetails.Type switch {
			DatabaseType.InMemory => new InMemoryDatabaseHandler(),
			DatabaseType.SQLite => new SQLiteDatabaseHandler(databaseDetails),
			_ => throw new IndexOutOfRangeException($"{databaseDetails.Type} is not a valid database type.")
		};
	}
}