using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OpenAi.Exceptions;
using OpenAi.Gpt;
using OpenAi.Images;
using OpenAi.Models;

namespace OpenAi;

public class OpenApi {
	private const string _apiUrl = "https://api.openai.com/v1";
	private readonly HttpClient _httpClient;

	public OpenApi(string apiKey) {
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
	}

	private static string ChatCompletionUrl => $"{_apiUrl}/chat/completions";
	private static string ImageGenerationUrl => $"{_apiUrl}/images/generations";

	public GptModel ChatModel { get; set; } = GptModel.Gpt_35_Turbo;
	public ImageModel ImageModel { get; set; } = ImageModel.Dall_E_2;

	public async Task<ChatGptResponse> GetChatGptResponseAsync(Message[] messages,
															   IEnumerable<GptFunction>? functions = null,
															   ToolChoice? toolChoice = null) {
		return await PostRequest<ChatGptResponse>(ChatCompletionUrl, new ChatGptRequest {
			Model = ChatModel,
			Messages = messages,
			Tools = functions?.Select(f => new GptTool {
				Function = f
			}).ToArray(),
			ToolChoice = toolChoice,
			MaxTokens = 1000
		});
	}

	public async Task<ImageResponse> GenerateImageAsync(string prompt, ImageSize size, ResponseFormat format) =>
		await PostRequest<ImageResponse>(ImageGenerationUrl, new ImageRequest {
			Model = ImageModel,
			Prompt = prompt,
			Size = size,
			ResponseFormat = format
		});

	private async Task<TResponse> PostRequest<TResponse>(string url, object requestBody) {
		var jsonContent = JsonSerializer.Serialize(requestBody);
		var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync(url, content);
		if (response.StatusCode == HttpStatusCode.BadRequest) {
			var badResponse = await response.Content.ReadAsStringAsync();
			var errorBody = JsonSerializer.Deserialize<ChatGptResponse>(badResponse)?.Error
							?? throw new NullReferenceException();
			throw new BadRequestException(errorBody, requestBody);
		}

		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();

		return JsonSerializer.Deserialize<TResponse>(responseBody)
			   ?? throw new NullReferenceException();
	}
}