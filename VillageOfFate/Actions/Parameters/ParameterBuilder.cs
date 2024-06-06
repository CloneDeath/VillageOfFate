using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace VillageOfFate.Actions.Parameters;

public static class ParameterBuilder {
	public static JsonObject GenerateJsonSchema<T>() => GenerateJsonSchema(typeof(T));

	public static JsonObject GenerateJsonSchema(Type type) {
		var jsonObject = new JsonObject();

		foreach (var property in type.GetProperties()) {
			var propertyName = GetPropertyName(property);
			var description = property.GetCustomAttribute<JsonDescriptionAttribute>()?.Description;
			var propertyType = property.PropertyType;
			var required = property.GetCustomAttribute<JsonRequiredAttribute>() != null;

			var jsonData = propertyType.Name switch {
				"String" => new JsonString {
					Description = description ??
								  throw new Exception(
									  $"{nameof(JsonDescriptionAttribute)} is required for string properties")
				},
				_ => throw new NotSupportedException($"Unsupported property type: {propertyType.Name}")
			};
			jsonObject.Properties.Add(propertyName, jsonData);

			if (required) {
				jsonObject.Required.Add(propertyName);
			}
		}

		return jsonObject;
	}

	private static string GetPropertyName(PropertyInfo property) {
		var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
		return jsonPropertyAttribute?.Name ?? property.Name;
	}
}