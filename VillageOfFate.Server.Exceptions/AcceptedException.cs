using System.Net;

namespace VillageOfFate.Server.Exceptions;

public class AcceptedException : HttpStatusException {
	public AcceptedException() { }
	public AcceptedException(string message) : base(message) { }
	public override HttpStatusCode StatusCode => HttpStatusCode.Accepted;
}

public class AcceptedException<TBody>(TBody content) : HttpStatusException<TBody>(content) {
	public override HttpStatusCode StatusCode => HttpStatusCode.Accepted;
}