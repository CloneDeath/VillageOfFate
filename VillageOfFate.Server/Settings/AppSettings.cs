namespace VillageOfFate.Server.Settings;

public class AppSettings {
	public DatabaseDetails Database { get; set; } = new();
	public bool GenerateImages { get; set; } = true;
}