using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace VillageOfFate.Actions.Parameters;

public static class ParameterBuilder {
	public static JsonObject GenerateJsonSchema<T>() => GenerateJsonSchema(typeof(T));

	public static JsonObject GenerateJsonSchema(Type type) {
		var jsonObject = new JsonObject();

		foreach (var property in type.GetProperties()) {
			var propertyName = GetPropertyName(property);
			var description = property.GetCustomAttribute<JsonDescriptionAttribute>()?.Description
							  ?? throw new Exception(
								  $"{nameof(JsonDescriptionAttribute)} is required for all properties. Missing on {propertyName} in {type.Name}.");
			var propertyType = property.PropertyType;
			var required = property.GetCustomAttribute<JsonRequiredAttribute>() != null;

			var jsonData = propertyType switch {
				{ Name: "String" } => (IJsonData)new JsonString {
					Description = description
				},
				{ Name: "Int32" } => new JsonNumber {
					Description = description
				},
				{ Name: "Double" } => new JsonNumber {
					Description = description
				},
				{ IsEnum: true } => new JsonEnum {
					Description = description,
					Enum = Enum.GetNames(propertyType).ToList()
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