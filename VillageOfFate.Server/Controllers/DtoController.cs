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
			result.Add($"TABLE {entityType.GetTableName()} ({entityType.ClrType.Name})");

			var properties = entityType.GetProperties().ToList();
			if (properties.Any()) {
				result.Add("\tProperties:");
				foreach (var property in properties) {
					var propertyType = property.IsNullable
										   ? $"{property.ClrType.GenericTypeArguments.FirstOrDefault()?.Name ?? property.ClrType.Name}?"
										   : $"{property.ClrType.Name}";
					var hostField = property.PropertyInfo == null ? null : $"Field: {property.PropertyInfo.Name}";
					var shadow = property.IsShadowProperty() ? "(Shadow) " : "";
					result.Add(
						$"\t\t{shadow}{property.GetColumnType()} {property.Name} ({propertyType}) -> {hostField ?? "Nav?"}");
				}
			}

			foreach (var props in entityType.GetComplexProperties()) {
				result.Add($"\tCOMPLEX{props.Name}");
			}

			var navigations = entityType.GetNavigations().ToList();
			if (navigations.Any()) {
				result.Add("\tNavigation Properties:");
				foreach (var props in entityType.GetNavigations()) {
					var shadow = props.IsShadowProperty() ? "(Shadow) " : "";
					var array = props.IsCollection ? "[]" : "";
					result.Add($"\t\t{shadow}{props.TargetEntityType.ClrType.Name}{array} {props.Name}");
				}
			}

			var foreignKeys = entityType.GetForeignKeys().ToList();
			if (foreignKeys.Count == 0) continue;

			result.Add("\tForeign Keys:");
			foreach (var fk in entityType.GetForeignKeys()) {
				var key = fk.Properties.Count == 1
							  ? fk.Properties.First().Name
							  : $"[ {string.Join(", ", fk.Properties.Select(x => x.Name))} ]";
				var outNavigation = fk.GetNavigation(true);
				var outNav = outNavigation == null ? null : $"<{outNavigation.Name}>";
				var inNavigation = fk.GetNavigation(false);
				var inNav = inNavigation == null ? null : $">{inNavigation.DeclaringEntityType} {inNavigation.Name}<";
				result.Add(
					$"\t\t{key} {fk.PrincipalEntityType.GetTableName()} ({fk.PrincipalEntityType.DisplayName()}) => {outNav ?? inNav ?? "????"}");
			}
		}

		return string.Join(Environment.NewLine, result);
	}
}