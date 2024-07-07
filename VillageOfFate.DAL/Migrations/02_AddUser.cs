using FluentMigrator;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SouthernCrm.Dal.Migrations;

[Migration(2024_07_06_06_00_00)]
public class AddUser : Migration {
	public const int MaxEmailAddressLength = 256;
	public override void Up() {
		Create.Table("Users")
			  .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
			  .WithColumn("EmailAddress").AsString(MaxEmailAddressLength).NotNullable()
			  .WithColumn("BibleId").AsGuid().Nullable();
	}

	public override void Down() {
		Delete.Table("Users");
	}
}