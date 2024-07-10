using System.Net;

namespace VillageOfFate.Server.Exceptions.NotFound;

public class NotFoundException : HttpStatusException {
	public NotFoundException() { }
	public NotFoundException(string message) : base(message) { }
	public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}

public class NotFoundException<TBody>(TBody content) : HttpStatusException<TBody>(content) {
	public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}