using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAi.Conversion;

public class JsonPropertyNameEnumConverter<T> : JsonConverter<T> where T : Enum {
	public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var enumString = reader.GetString();
		foreach (var field in typeToConvert.GetFields()) {
			var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
			if (attribute == null) continue;
			if (attribute.Name != enumString) continue;
			return (T)field.GetValue(null)!;
		}

		throw new JsonException($"Unable to convert \"{enumString}\" to enum {typeToConvert}.");
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		foreach (var field in value.GetType().GetFields()) {
			var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
			if (attribute == null) continue;
			if (!field.GetValue(null)!.Equals(value)) continue;
			writer.WriteStringValue(attribute.Name);
			return;
		}

		throw new JsonException($"Unable to convert enum {value.GetType()} to string.");
	}
}