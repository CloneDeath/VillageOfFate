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
using OpenAi.Models;

namespace OpenAi;

public class ChatGptApi {
	private readonly HttpClient _httpClient;
	private const string _apiUrl = "https://api.openai.com/v1";
	private string ChatCompletionUrl => $"{_apiUrl}/chat/completions";

	public GptModel Model { get; set; } = GptModel.Gpt_35_Turbo;

	public ChatGptApi(string apiKey) {
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
	}

	public async Task<ChatGptResponse> GetChatGptResponseAsync(Message[] messages, IEnumerable<GptFunction>? functions = null, ToolChoice? toolChoice = null) {
		var requestBody = new ChatGptRequest {
			Model = Model.ToModelString(),
			Messages = messages,
			Tools = functions?.Select(f => new GptTool {
				Function = f
			}).ToArray(),
			ToolChoice = toolChoice,
			MaxTokens = 1000
		};

		var jsonContent = JsonSerializer.Serialize(requestBody);
		var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

		var response = await _httpClient.PostAsync(ChatCompletionUrl, content);
		if (response.StatusCode == HttpStatusCode.BadRequest) {
			var badResponse = await response.Content.ReadAsStringAsync();
			var errorBody = JsonSerializer.Deserialize<ChatGptResponse>(badResponse)?.Error
							?? throw new NullReferenceException();
			throw new BadRequestException(errorBody, requestBody);
		}
		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<ChatGptResponse>(responseBody)
							 ?? throw new NullReferenceException();
	}
}