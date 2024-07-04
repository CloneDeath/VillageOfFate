using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VillageOfFate.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase {
	[HttpGet("Me")]
	public string GetInfoAboutMe() {
		List<string> result = [];

		var user = HttpContext.User;
		foreach (var id in user.Identities) {
			result.Add($"User: {id.Name}");
			result.Add($"Is Authenticated: {id.IsAuthenticated}");
			result.Add($"Authentication Type: {id.AuthenticationType}");
			foreach (var claim in id.Claims) {
				result.Add($"\t{claim.Type}: {claim.Value} ({claim.ValueType})");
				foreach (var claimProperty in claim.Properties) {
					result.Add($"\t\t{claimProperty.Key}: {claimProperty.Value}");
				}
			}
		}

		return string.Join(Environment.NewLine, result);
	}
}