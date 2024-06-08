using System.Text.Json.Serialization;

namespace OpenAi.Images;

public class ImageResponse {
	[JsonPropertyName("created")] public long Created { get; set; }
	[JsonPropertyName("data")] public ImageData[] Data { get; set; } = [];
}

public class ImageData {
	[JsonPropertyName("b64_json")] public string? b64_json { get; set; }
	[JsonPropertyName("url")] public string? url { get; set; }
	[JsonPropertyName("revised_prompt")] public string? revised_prompt { get; set; }
}