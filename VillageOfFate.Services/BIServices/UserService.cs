using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.BIServices;

public class UserService(IHttpContextAccessor httpContextAccessor, DataContext context) {
	public string? GetEmailAddress() {
		var httpContext = httpContextAccessor.HttpContext;
		var claim = httpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
		return claim?.Value;
	}

	public async Task<UserDto> GetUserAsync() {
		var emailAddress = GetEmailAddress();
		if (string.IsNullOrEmpty(emailAddress)) {
			throw new Exception("No email address found in claims.");
		}

		var user = await context.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
		if (user != null) return user;

		var result = await context.Users.AddAsync(new UserDto {
			EmailAddress = emailAddress
		});
		return result.Entity;
	}

	public async Task SaveAsync(UserDto user) {
		context.Users.Update(user);
		await context.SaveChangesAsync();
	}
}