using System;

namespace VillageOfFate.WebModels;

public class WebUser {
	public required Guid Id { get; init; }
	public required string EmailAddress { get; init; }
	public required Guid? BibleId { get; init; }
}