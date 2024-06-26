using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DtoController(DataContext context) : ControllerBase {
	[HttpGet]
	public string GetModelDescription() {
		List<string> result = [];

		var model = context.Model;
		foreach (var entityType in model.GetEntityTypes()) {
			result.Add($"TABLE {entityType.GetTableName()} ({entityType.Name})");

			foreach (var property in entityType.GetProperties()) {
				result.Add(
					$"\t{property.GetColumnType()} {property.Name} ({property.ClrType.Name}{(property.IsNullable ? "?" : "")})");
			}

			var foreignKeys = entityType.GetForeignKeys().ToList();
			if (foreignKeys.Count == 0) continue;

			result.Add("\tForeign Keys:");
			foreach (var fk in entityType.GetForeignKeys()) {
				result.Add(
					$"\t\t[ {string.Join(", ", fk.Properties.Select(x => x.Name))} ] {fk.PrincipalEntityType.GetTableName()} ({fk.PrincipalEntityType.DisplayName()})");
			}
		}

		return string.Join(Environment.NewLine, result);
	}
}