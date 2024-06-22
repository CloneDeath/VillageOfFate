using Microsoft.EntityFrameworkCore;

namespace VillageOfFate.DAL;

// ReSharper disable once InconsistentNaming
public class SQLiteDataContext(string dataSource) : DataContext(BuildOptions(dataSource)) {
	private static DbContextOptions<DataContext> BuildOptions(string dataSource) {
		var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
		ConfigureOptionsBuilder(optionsBuilder, dataSource);
		return optionsBuilder.Options;
	}

	public static void ConfigureOptionsBuilder(DbContextOptionsBuilder builder, string dataSource) {
		builder.UseSqlite($"Data Source={dataSource}",
			o => o.CommandTimeout(30));
	}
}