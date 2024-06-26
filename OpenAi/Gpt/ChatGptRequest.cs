using System.Text.Json.Serialization;
using OpenAi.Conversion;
using OpenAi.Models;

namespace OpenAi.Gpt;

public class ChatGptRequest {
	[JsonPropertyName("model")] public GptModel Model { get; set; } = GptModel.Gpt_4_Omni;
	[JsonPropertyName("messages")] public Message[] Messages { get; set; } = [];
	[JsonPropertyName("max_tokens")] public int? MaxTokens { get; set; }
	[JsonPropertyName("tools")] public GptTool[]? Tools { get; set; }
	[JsonPropertyName("tool_choice")] public ToolChoice? ToolChoice { get; set; }
}

public class GptTool {
	[JsonPropertyName("type")] public string Type { get; set; } = "function";
	[JsonPropertyName("function")] public GptFunction Function { get; set; } = new();
}

public class GptFunction {
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("description")] public string? Description { get; set; }
	[JsonPropertyName("parameters")] public object? Parameters { get; set; }
}

[JsonConverter(typeof(JsonPropertyNameEnumConverter<ToolChoice>))]
public enum ToolChoice {
	[JsonPropertyName("none")] None,
	[JsonPropertyName("auto")] Auto,
	[JsonPropertyName("required")] Required
}