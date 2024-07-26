using System;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.SqlServer;
using VillageOfFate.DAL.Entities;
using VillageOfFate.DAL.Entities.Activities;
using VillageOfFate.WebModels;

namespace SouthernCrm.Dal.Migrations {
	[Migration(2024_05_27_06_15_00)]
	public class InitialCreate : Migration {
		public const int MaxDescriptionLength = 4096;
		public const int MaxNameLength = 1024;

		public override void Up() {
			Create.Table("Time")
				  .WithColumn("Label").AsString(MaxNameLength).NotNullable().PrimaryKey()
				  .WithColumn("Time").AsDateTime().NotNullable();

			Create.Table("ItemDefinitions")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Description").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Category").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Edible").AsBoolean().NotNullable()
				  .WithColumn("HungerRestored").AsInt32().NotNullable()
				  .WithColumn("ImageId").AsGuid().Nullable().ForeignKey("Images", "Id");

			Create.Table("Items")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("ItemDefinitionId").AsGuid().NotNullable().ForeignKey("ItemDefinitions", "Id")
				  .WithColumn("Quantity").AsInt32().NotNullable()
				  .WithColumn("CreationDate").AsDateTime().Nullable()
				  .WithColumn("PageNumber").AsInt32().Nullable()
				  .WithColumn("Content").AsString(MaxDescriptionLength).Nullable()
				  .WithColumn("VillagerId").AsGuid().Nullable().ForeignKey("Villagers", "Id")
				  .WithColumn("SectorId").AsGuid().Nullable().ForeignKey("Sectors", "Id")
				  .WithColumn("ItemId").AsGuid().Nullable().ForeignKey("Items", "Id");

			Create.Table("Activities")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Status").AsString(MaxNameLength).NotNullable().WithDefaultValue(nameof(ActivityStatus.Pending))
				  .WithColumn("Arguments").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Priority").AsInt32().NotNullable().WithDefaultValue(int.MaxValue)
				  .WithColumn("StartTime").AsDateTime().Nullable()
				  .WithColumn("TotalDuration").AsTime().NotNullable()
				  .WithColumn("DurationRemaining").AsTime().NotNullable()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  // Adjust Emotional State
				  .WithColumn("Emotion").AsString(MaxNameLength).Nullable()
				  .WithColumn("Adjustment").AsInt32().Nullable()
				  .WithColumn("Reason").AsString(MaxDescriptionLength).Nullable()
				  // Speak
				  .WithColumn("Content").AsString(MaxDescriptionLength).Nullable()
				  // Interact
				  .WithColumn("Action").AsString(MaxDescriptionLength).Nullable()
				  // Eat
				  .WithColumn("TargetItemId").AsGuid().Nullable().ForeignKey("Items", "Id");
			Create.Table("InteractActivityTargets")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("ActivityId").AsGuid().NotNullable().ForeignKey("Activities", "Id")
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id");
			Create.UniqueConstraint()
				  .OnTable("InteractActivityTargets")
				  .Columns(["ActivityId", "VillagerId"]);

			Create.Table("Villagers")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Age").AsInt32().NotNullable()
				  .WithColumn("Summary").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Gender").AsInt32().NotNullable().ForeignKey("Genders", "Id")
				  .WithColumn("Hunger").AsInt32().NotNullable()
				  .WithColumn("SectorId").AsGuid().NotNullable().ForeignKey("Sectors", "Id")
				  .WithColumn("EmotionsId").AsGuid().NotNullable().ForeignKey("Emotions", "Id")
				  .WithColumn("ImageId").AsGuid().Nullable().ForeignKey("Images", "Id");

			Create.Table("Genders")
				  .WithColumn("Id").AsInt32().NotNullable().PrimaryKey()
				  .WithColumn("Name").AsString().NotNullable();
			Insert.IntoTable("Genders")
				  .Row(new { Id = (int)Gender.Male, Name = "Male" })
				  .Row(new { Id = (int)Gender.Female, Name = "Female" });

			Create.Table("Emotions")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Happiness").AsInt32().NotNullable()
				  .WithColumn("Sadness").AsInt32().NotNullable()
				  .WithColumn("Fear").AsInt32().NotNullable();

			Create.Table("Events")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Description").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Time").AsDateTime().NotNullable()
				  .WithColumn("Order").AsInt32().NotNullable()
				  .WithColumn("SectorId").AsGuid().NotNullable().ForeignKey("Sectors", "Id")
				  .WithColumn("VillagerActorId").AsGuid().Nullable().ForeignKey("Villagers", "Id")
				  .WithColumn("ItemActorId").AsGuid().Nullable().ForeignKey("Items", "Id");

			Create.Table("EventWitnesses")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("EventId").AsGuid().NotNullable().ForeignKey("Events", "Id")
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id");
			Create.UniqueConstraint()
				  .OnTable("EventWitnesses")
				  .Columns("EventId", "VillagerId");

			Create.Table("Sectors")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Description").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("X").AsInt32().NotNullable()
				  .WithColumn("Y").AsInt32().NotNullable()
				  .WithColumn("ImageId").AsGuid().Nullable().ForeignKey("Images", "Id");
			Create.UniqueConstraint()
				  .OnTable("Sectors")
				  .Columns("X", "Y");

			Create.Table("Relationships")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("RelationId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("Summary").AsString(MaxDescriptionLength).NotNullable();
			Create.UniqueConstraint()
				  .OnTable("Relationships")
				  .Columns("VillagerId", "RelationId");

			Create.Table("GptUsage")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("WorldTime").AsDateTime().NotNullable()
				  .WithColumn("EarthTime").AsDateTime().NotNullable()
				  .WithColumn("TotalTokens").AsInt32().NotNullable()
				  .WithColumn("PromptTokens").AsInt32().NotNullable()
				  .WithColumn("CompletionTokens").AsInt32().NotNullable();

			Create.Table("VillagerActionErrors")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("WorldTime").AsDateTime().NotNullable()
				  .WithColumn("EarthTime").AsDateTime().NotNullable()
				  .WithColumn("VillagerId").AsGuid().NotNullable().ForeignKey("Villagers", "Id")
				  .WithColumn("ActivityId").AsGuid().Nullable().ForeignKey("Activities", "Id")
				  .WithColumn("ActionName").AsString(MaxNameLength).NotNullable()
				  .WithColumn("Arguments").AsString(MaxDescriptionLength).NotNullable()
				  .WithColumn("Error").AsString(MaxDescriptionLength).NotNullable();

			Create.Table("Images")
				  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
				  .WithColumn("Created").AsDateTime().Nullable()
				  .WithColumn("Base64Image").AsString(ImageDto.MaxBase64ImageLength).Nullable()
				  .WithColumn("Prompt").AsString(MaxDescriptionLength).Nullable();

		}

		public override void Down() {
			Delete.Table("Activities");
			Delete.Table("Emotions");
			Delete.Table("Events");
			Delete.Table("EventWitnesses");
			Delete.Table("Genders");
			Delete.Table("GptUsage");
			Delete.Table("Images");
			Delete.Table("InteractActivityTargets");
			Delete.Table("ItemDefinitions");
			Delete.Table("Items");
			Delete.Table("Relationships");
			Delete.Table("SectorItems");
			Delete.Table("Sectors");
			Delete.Table("Time");
			Delete.Table("VillagerActionErrors");
			Delete.Table("VillagerItems");
			Delete.Table("Villagers");
		}
	}
}