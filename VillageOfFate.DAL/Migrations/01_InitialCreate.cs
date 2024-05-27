using System;
using FluentMigrator;
using FluentMigrator.SqlServer;
using VillageOfFate.DAL.Entities;

namespace SouthernCrm.Dal.Migrations {
	[Migration(2024_05_27_06_15_00)]
	public class InitialCreate : Migration {
		public override void Up() {
			Create.Table("Time")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Now").AsDateTime().NotNullable();

			Create.Table("Villagers")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString().NotNullable()
				  .WithColumn("Age").AsInt32().NotNullable()
				  .WithColumn("Summary").AsInt32().NotNullable()
				  .WithColumn("Gender").AsInt32().NotNullable().ForeignKey("Genders", "Id")
				  .WithColumn("Hunger").AsInt32().NotNullable();

			Create.Table("Genders")
				  .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString().NotNullable();
			Insert.IntoTable("Genders")
				  .Row(new { Id = (int)Gender.Male, Name = "Male" })
				  .Row(new { Id = (int)Gender.Female, Name = "Female" });

			Create.Table("Items")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString().NotNullable()
				  .WithColumn("Description").AsString().NotNullable()
				  .WithColumn("Quantity").AsInt32().NotNullable()
				  .WithColumn("Edible").AsBoolean().NotNullable()
				  .WithColumn("HungerRestored").AsInt32().NotNullable();

			Create.Table("VillagerItems")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("ItemId").AsGuid().NotNullable().ForeignKey("Items", "Id");
			Create.UniqueConstraint()
				  .OnTable("VillagerItems")
				  .Columns("VillagerId", "ItemId");
		}

		public override void Down() {
			Delete.Table("Time");
			Delete.Table("Villagers");
			Delete.Table("Genders");
			Delete.Table("Items");
			Delete.Table("VillagerItems");
		}
	}
}