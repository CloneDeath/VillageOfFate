using System;
using System.Text.Json;

namespace GptApi.Exceptions;

public class BadRequestException(ErrorResponse response, ChatGptRequest request)
	: Exception($"BadRequest: {JsonSerializer.Serialize(response)}, Request: {JsonSerializer.Serialize(request)}") {
	public ErrorResponse Response { get; } = response;
	public ChatGptRequest Request { get; } = request;
}