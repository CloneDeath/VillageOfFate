using System;
using FluentMigrator;
using VillageOfFate.DAL.Entities;

namespace SouthernCrm.Dal.Migrations {
	[Migration(2024_05_27_06_15_00)]
	public class InitialCreate : Migration {
		public override void Up() {
			Create.Table("Time")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().Identity()
				  .WithColumn("Now").AsDateTime().NotNullable();

			Create.Table("Villager")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey().Identity()
				  .WithColumn("Name").AsString().NotNullable()
				  .WithColumn("Age").AsInt32().NotNullable()
				  .WithColumn("Summary").AsInt32().NotNullable()
				  .WithColumn("Gender").AsInt32().NotNullable().ForeignKey("Gender", "Id")
				  .WithColumn("Hunger").AsInt32().NotNullable();

			Create.Table("Gender")
				  .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				  .WithColumn("Name").AsString().NotNullable();

			Insert.IntoTable("Gender")
				  .Row(new { Id = (int)Gender.Male, Name = "Male" })
				  .Row(new { Id = (int)Gender.Female, Name = "Female" });
		}

		public override void Down() {
			Delete.Table("Time");
			Delete.Table("Villager");
			Delete.Table("Gender");
		}
	}
}