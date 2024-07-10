using Microsoft.AspNetCore.Http;

namespace VillageOfFate.Server.Exceptions;

public class ExceptionHandler : IMiddleware {
	public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
		try {
			await next.Invoke(context);
		}
		catch (HttpStatusException ex) {
			if (context.Response.HasStarted) throw;

			context.Response.Clear();
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)ex.StatusCode;
			await context.Response.WriteAsync(ex.Content);
		}
	}
}