using System;
using FluentMigrator;
using FluentMigrator.SqlServer;
using VillageOfFate.DAL.Entities;
using VillageOfFate.WebModels;

namespace SouthernCrm.Dal.Migrations {
	[Migration(2024_05_27_06_15_00)]
	public class InitialCreate : Migration {
		public const int MaxDescriptionLength = 4096;
		public const int MaxNameLength = 1024;

		public override void Up() {
			Create.Table("Time")
				  .WithColumn("Label").AsString(MaxNameLength).NotNullable().PrimaryKey()
				  .WithColumn("Now").AsDateTime().NotNullable();

			Create.Table("Items")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Description").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Quantity").AsInt32().NotNullable()
				  .WithColumn("Edible").AsBoolean().NotNullable()
				  .WithColumn("HungerRestored").AsInt32().NotNullable();

			Create.Table("Activities")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString(MaxNameLength).NotNullable()
				  .WithColumn("StartTime").AsDateTime().NotNullable()
				  .WithColumn("Description").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("DurationTicks").AsInt64().NotNullable()
				  .WithColumn("Interruptible").AsBoolean().NotNullable();

			Create.Table("Villagers")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Age").AsInt32().NotNullable()
				  .WithColumn("Summary").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Gender").AsInt32().NotNullable().ForeignKey("Genders", "Id")
				  .WithColumn("Hunger").AsInt32().NotNullable()
				  .WithColumn("SectorId").AsGuid().NotNullable().ForeignKey("Sectors", "Id")
				  .WithColumn("ActivityId").AsGuid().NotNullable().Unique().ForeignKey("Activities", "Id");

			Create.Table("Genders")
				  .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString().NotNullable();
			Insert.IntoTable("Genders")
				  .Row(new { Id = (int)Gender.Male, Name = "Male" })
				  .Row(new { Id = (int)Gender.Female, Name = "Female" });

			Create.Table("VillagerItems")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("ItemId").AsGuid().NotNullable().ForeignKey("Items", "Id");
			Create.UniqueConstraint()
				  .OnTable("VillagerItems")
				  .Columns("VillagerId", "ItemId");

			Create.Table("VillagerMemories")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("Memory").AsString(MaxDescriptionLength).NotNullable();

			Create.Table("Sectors")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Description").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("X").AsInt32().NotNullable()
				  .WithColumn("Y").AsInt32().NotNullable();
			Create.UniqueConstraint()
				  .OnTable("Sectors")
				  .Columns("X", "Y");

			Create.Table("SectorItems")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("SectorId").AsGuid().NotNullable().ForeignKey("Sectors", "Id")
				  .WithColumn("ItemId").AsGuid().NotNullable().ForeignKey("Items", "Id");
			Create.UniqueConstraint()
				  .OnTable("SectorItems")
				  .Columns("SectorId", "ItemId");

			Create.Table("Relationships")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("RelationId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("Summary").AsString(MaxDescriptionLength).NotNullable();
			Create.UniqueConstraint()
				  .OnTable("Relationships")
				  .Columns("VillagerId", "RelationId");
		}

		public override void Down() {
			Delete.Table("Time");
			Delete.Table("Villagers");
			Delete.Table("Genders");
			Delete.Table("Items");
			Delete.Table("VillagerItems");
			Delete.Table("Sectors");
			Delete.Table("SectorItems");
		}
	}
}