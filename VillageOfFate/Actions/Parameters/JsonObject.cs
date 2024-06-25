using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VillageOfFate.Actions.Parameters;

public interface IJsonData {
	string Type { get; }
}

public class JsonObject : IJsonData {
	[JsonPropertyName("properties")] public virtual Dictionary<string, IJsonData> Properties { get; init; } = new();
	[JsonPropertyName("required")] public virtual List<string> Required { get; init; } = [];
	[JsonPropertyName("type")] public string Type => "object";
}

public class JsonString : IJsonData {
	[JsonPropertyName("description")] public required string Description { get; init; }
	[JsonPropertyName("format")] public string? Format { get; set; }
	[JsonPropertyName("type")] public string Type => "string";
}

public class JsonNumber : IJsonData {
	[JsonPropertyName("description")] public required string Description { get; init; }
	[JsonPropertyName("type")] public string Type => "number";
}

public class JsonEnum : IJsonData {
	[JsonPropertyName("enum")] public required List<string> Enum { get; init; }
	[JsonPropertyName("description")] public required string Description { get; init; }
	[JsonPropertyName("type")] public string Type => "string";
}

public class JsonArray : IJsonData {
	[JsonPropertyName("description")] public required string Description { get; init; }
	[JsonPropertyName("items")] public required IJsonData Items { get; set; }
	[JsonPropertyName("type")] public string Type => "array";
}