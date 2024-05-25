using System.Text.Json.Serialization;

namespace VillageOfFate.WebModels;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender {
	Male,
	Female
}