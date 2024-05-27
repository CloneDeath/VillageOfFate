using FluentMigrator;

namespace SouthernCrm.Dal.Migrations {
	[Migration(2024_05_27_06_15_00)]
	public class InitialCreate : Migration {
		public override void Up() {
			Create.Table("Time")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().Identity()
				  .WithColumn("Now").AsDateTime().NotNullable();
		}

		public override void Down() {
			Delete.Table("Time");
		}
	}
}