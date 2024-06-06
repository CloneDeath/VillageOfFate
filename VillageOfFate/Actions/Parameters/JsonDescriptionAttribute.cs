using System;

namespace VillageOfFate.Actions.Parameters;

[AttributeUsage(AttributeTargets.Property)]
public class JsonDescriptionAttribute(string description) : Attribute {
	public string Description => description;
}