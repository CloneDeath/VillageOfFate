using System.Net;
using System.Text.Json;

namespace VillageOfFate.Server.Exceptions;

public abstract class HttpStatusException : Exception {
	protected HttpStatusException() { }

	protected HttpStatusException(string message) {
		Content = JsonSerializer.Serialize(new ErrorMessage {
			Message = message
		});
	}

	public abstract HttpStatusCode StatusCode { get; }
	public virtual string Content { get; } = string.Empty;
}

public abstract class HttpStatusException<TBody>(TBody content) : HttpStatusException {
	public override string Content => JsonSerializer.Serialize(ContentObject);
	public TBody ContentObject { get; } = content;
}