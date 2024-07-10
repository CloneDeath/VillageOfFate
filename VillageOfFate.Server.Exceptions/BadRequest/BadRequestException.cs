using System.Net;

namespace VillageOfFate.Server.Exceptions.BadRequest;

public class BadRequestException : HttpStatusException {
	public BadRequestException() { }
	public BadRequestException(string message) : base(message) { }
	public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}

public class BadRequestException<TBody>(TBody content) : HttpStatusException<TBody>(content) {
	public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}