using System;
using System.ComponentModel.DataAnnotations;
using SouthernCrm.Dal.Migrations;

namespace VillageOfFate.DAL.Entities;

public class UserDto {
	public Guid Id { get; set; }
	[MaxLength(AddUser.MaxEmailAddressLength)]
	[EmailAddress]
	public string EmailAddress { get; set; } = string.Empty;
}