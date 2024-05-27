using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;

namespace VillageOfFate.Server.Databases;

public class InMemoryDatabaseHandler : IDatabaseHandler {
	public void BuildContext(DbContextOptionsBuilder builder) {
		InMemoryDataContext.ConfigureOptionsBuilder(builder, "SouthernCRM");
	}

	public void RunMigration() { }
}