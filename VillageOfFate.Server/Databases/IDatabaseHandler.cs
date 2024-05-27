using Microsoft.EntityFrameworkCore;

namespace VillageOfFate.Server.Databases;

public interface IDatabaseHandler {
	void BuildContext(DbContextOptionsBuilder builder);
	void RunMigration();
}