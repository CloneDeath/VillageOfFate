using System.Net;

namespace VillageOfFate.Server.Exceptions.Unauthorized;

public class UnauthorizedException : HttpStatusException {
	public UnauthorizedException() { }
	public UnauthorizedException(string message) : base(message) { }
	public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}

public class UnauthorizedException<TBody>(TBody content) : HttpStatusException<TBody>(content) {
	public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}