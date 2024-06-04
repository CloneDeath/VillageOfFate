using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.DAL;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DtoController(DataContext context) : ControllerBase {
	[HttpGet]
	public string GetAll() {
		List<string> result = [];

		var model = context.Model;
		foreach (var entityType in model.GetEntityTypes()) {
			result.Add($"Entity: {entityType.Name}");

			foreach (var property in entityType.GetProperties()) {
				result.Add($"\tProperty: {property.Name}");
			}

			foreach (var fk in entityType.GetForeignKeys()) {
				result.Add(
					$"\tForeign Key: {fk.Properties.Select(x => x.Name)} References {fk.PrincipalEntityType.Name}");
			}
		}

		return string.Join(Environment.NewLine, result);
	}
}