using System.Text.Json.Serialization;
using OpenAi.Gpt;

namespace OpenAi.Models;

[JsonConverter(typeof(LowerCaseEnumConverter))]
public enum ImageModel {
	[JsonPropertyName("dall-e-2")] Dall_E_2,
	[JsonPropertyName("dall-e-3")] Dall_E_3
}