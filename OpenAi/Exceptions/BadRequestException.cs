using System;
using System.Text.Json;

namespace OpenAi.Exceptions;

public class BadRequestException(ErrorResponse response, object request)
	: Exception($"BadRequest: {JsonSerializer.Serialize(response)}, Request: {JsonSerializer.Serialize(request)}") {
	public ErrorResponse Response { get; } = response;
	public object Request { get; } = request;
}