using System.Text.Json.Serialization;

namespace VillageOfFate.Server.Exceptions;

public class ErrorMessage {
	[JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}