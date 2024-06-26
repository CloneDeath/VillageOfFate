using System.Text.Json.Serialization;
using OpenAi.Conversion;

namespace OpenAi.Gpt;

public class Message {
	[JsonPropertyName("role")] public Role Role { get; set; } = Role.User;
	[JsonPropertyName("content")] public string? Content { get; set; } = string.Empty;

	[JsonPropertyName("name")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Name { get; set; }

	[JsonPropertyName("tool_calls")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public ToolCall[]? ToolCalls { get; set; }

	[JsonPropertyName("tool_call_id")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ToolCallId { get; set; }
}

public class ToolCall {
	[JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
	[JsonPropertyName("type")] public string Type { get; set; } = "function";
	[JsonPropertyName("function")] public FunctionCall Function { get; set; } = new();
}

public class FunctionCall {
	[JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
	[JsonPropertyName("arguments")] public string Arguments { get; set; } = string.Empty;
}

[JsonConverter(typeof(JsonPropertyNameEnumConverter<Role>))]
public enum Role {
	[JsonPropertyName("user")] User,
	[JsonPropertyName("system")] System,
	[JsonPropertyName("assistant")] Assistant,
	[JsonPropertyName("tool")] Tool
}