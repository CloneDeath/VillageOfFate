using Microsoft.EntityFrameworkCore;

namespace VillageOfFate.DAL;

public class InMemoryDataContext(string databaseName) : DataContext(BuildOptions(databaseName)) {
	private static DbContextOptions<DataContext> BuildOptions(string databaseName) {
		var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
		ConfigureOptionsBuilder(optionsBuilder, databaseName);
		return optionsBuilder.Options;
	}

	public static void ConfigureOptionsBuilder(DbContextOptionsBuilder builder, string databaseName) {
		builder.UseInMemoryDatabase(databaseName);
	}
}