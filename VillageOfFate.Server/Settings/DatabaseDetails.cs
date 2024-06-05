namespace VillageOfFate.Server.Settings;

public class DatabaseDetails {
	public DatabaseType Type { get; set; }
	public bool EnableSensitiveDataLogging { get; set; }

	/* SQLite */
	public string DataSource { get; set; } = string.Empty;
}

public enum DatabaseType {
	InMemory,
	// ReSharper disable once InconsistentNaming
	SQLite
}