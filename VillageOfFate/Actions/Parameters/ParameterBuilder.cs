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

			var jsonData = GetJsonSchemaFor(propertyType, description);
			jsonObject.Properties.Add(propertyName, jsonData);

			if (required) {
				jsonObject.Required.Add(propertyName);
			}
		}

		return jsonObject;
	}

	private static IJsonData GetJsonSchemaFor(Type type, string description) {
		return type switch {
			{ Name: "String" } => new JsonString {
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
				Enum = Enum.GetNames(type).ToList()
			},
			{ Name: "Guid" } => new JsonString {
				Description = description,
				Format = "uuid"
			},
			{ IsArray: true } => new JsonArray {
				Description = description,
				Items = GetJsonSchemaFor(type.GetElementType()
										 ?? throw new NullReferenceException("Found an array, without an element type"),
					description)
			},
			_ => throw new NotSupportedException($"Unsupported property type: {type.Name}")
		};
	}

	private static string GetPropertyName(PropertyInfo property) {
		var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
		return jsonPropertyAttribute?.Name ?? property.Name;
	}
}