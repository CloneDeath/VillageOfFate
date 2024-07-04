using Microsoft.AspNetCore.Http;

namespace VillageOfFate.Server.Services;

public class UserService(IHttpContextAccessor contextAccessor) {
	public string? GetEmailAddress() {
		var context = contextAccessor.HttpContext;
		var claim = context?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
		return claim?.Value;
	}
}