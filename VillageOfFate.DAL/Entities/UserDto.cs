using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;
using VillageOfFate.DAL.Entities.Items;

namespace VillageOfFate.DAL.Entities;

public class UserDto {
	public Guid Id { get; set; }
	[MaxLength(AddUser.MaxEmailAddressLength)]
	[EmailAddress]
	public string EmailAddress { get; set; } = string.Empty;

	public Guid? BibleId { get; set; }
	public ItemDto? Bible { get; set; }
}