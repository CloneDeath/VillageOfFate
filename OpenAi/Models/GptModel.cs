using System.Text.Json.Serialization;
using OpenAi.Gpt;

namespace OpenAi.Models;

[JsonConverter(typeof(LowerCaseEnumConverter))]
public enum GptModel {
	[JsonPropertyName("gpt-3.5-turbo")] Gpt_35_Turbo,
	[JsonPropertyName("gpt-4-turbo")] Gpt_4_Turbo,
	[JsonPropertyName("gpt-4o")] Gpt_4_Omni
}