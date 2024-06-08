using System.Text.Json.Serialization;
using OpenAi.Conversion;
using OpenAi.Models;

namespace OpenAi.Images;

public class ImageRequest {
	[JsonPropertyName("prompt")] public string Prompt { get; set; } = string.Empty;
	[JsonPropertyName("model")] public ImageModel Model { get; set; } = ImageModel.Dall_E_3;
	[JsonPropertyName("response_format")]
	public ResponseFormat ResponseFormat { get; set; } = ResponseFormat.Base64_Json;
	[JsonPropertyName("size")] public ImageSize Size { get; set; } = ImageSize._1024x1024;
}

[JsonConverter(typeof(JsonPropertyNameEnumConverter<ResponseFormat>))]
public enum ResponseFormat {
	[JsonPropertyName("url")] Url,
	[JsonPropertyName("b64_json")] Base64_Json
}

[JsonConverter(typeof(JsonPropertyNameEnumConverter<ImageSize>))]
public enum ImageSize {
	[JsonPropertyName("256x256")] _256x256,
	[JsonPropertyName("512x512")] _512x512,
	[JsonPropertyName("1024x1024")] _1024x1024,
	[JsonPropertyName("1792x1024")] _1792x1024,
	[JsonPropertyName("1024x1792")] _1024x1792
}