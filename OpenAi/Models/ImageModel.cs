using System.Text.Json.Serialization;
using OpenAi.Conversion;

namespace OpenAi.Models;

[JsonConverter(typeof(JsonPropertyNameEnumConverter<ImageModel>))]
public enum ImageModel {
	[JsonPropertyName("dall-e-2")] Dall_E_2,
	[JsonPropertyName("dall-e-3")] Dall_E_3
}