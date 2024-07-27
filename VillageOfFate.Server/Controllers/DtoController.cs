using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
					var fkInfo = GetForeignKeyInfo(props.ForeignKey);
					result.Add($"\t\t{shadow}{props.TargetEntityType.ClrType.Name}{array} {props.Name} => {fkInfo}");
				}
			}

			var foreignKeys = entityType.GetForeignKeys().ToList();
			if (foreignKeys.Count == 0) continue;

			result.Add("\tForeign Keys:");
			foreach (var fk in entityType.GetForeignKeys()) {
				var fkProperties = fk.Properties.ToList();
				var key = fkProperties.Count == 1
							  ? fkProperties.First().Name
							  : $"[ {string.Join(", ", fkProperties.Select(x => x.Name))} ]";
				var fkInfo = GetForeignKeyInfo(fk);
				result.Add(
					$"\t\t{key} {fk.PrincipalEntityType.GetTableName()} ({fk.PrincipalEntityType.DisplayName()}) => {fkInfo}");
			}
		}

		return string.Join(Environment.NewLine, result);
	}

	private static string GetForeignKeyInfo(IForeignKey fk) {
		var tableName = fk.DeclaringEntityType.GetTableName();
		var fkProperties = fk.Properties.ToList();
		var tableColumns = fkProperties.Count == 1
							   ? fkProperties.First().GetColumnName()
							   : $"[ {string.Join(", ", fkProperties.Select(x => x.GetColumnName()))} ]";

		var outNavigation = fk.GetNavigation(true);
		var outNav = outNavigation == null ? null : $"<{outNavigation.Name}>";
		var inNavigation = fk.GetNavigation(false);
		var inNav = inNavigation == null ? null : $">{inNavigation.DeclaringEntityType.Name} {inNavigation.Name}<";
		return $"{outNav ?? inNav ?? "????"} @ {tableName}.{tableColumns}";
	}

	[HttpGet("model")]
	public string GetModelDebug() {
		List<string> result = [];

		var model = context.Model;
		result.Add(model.ToDebugString());
		return string.Join(Environment.NewLine, result);
	}

	[HttpGet("entity")]
	public string GetEntityDebugDescriptions() {
		List<string> result = [];

		var model = context.Model;
		foreach (var entityType in model.GetEntityTypes()) {
			result.Add($"{entityType.ToDebugString()}");
		}

		return string.Join(Environment.NewLine, result);
	}

	[HttpGet("debug")]
	public string GetDetailedDebugDescription() {
		List<string> result = [];

		var model = context.Model;
		foreach (var entityType in model.GetEntityTypes()) {
			result.Add($"{entityType.ToDebugString()}");

			var properties = entityType.GetProperties().ToList();
			if (properties.Any()) {
				result.Add("\tProperties:");
				result.AddRange(properties.Select(property => $"\t\t{property.ToDebugString()}"));
			}

			var complex = entityType.GetComplexProperties().ToList();
			if (complex.Any()) {
				result.Add("\tComplex:");
				result.AddRange(complex.Select(p => $"\t\t{p.ToDebugString()}"));
			}

			var navigations = entityType.GetNavigations().ToList();
			if (navigations.Any()) {
				result.Add("\tNavigation Properties:");
				result.AddRange(navigations.Select(p => $"\t\t{p.ToDebugString()}"));
			}

			var foreignKeys = entityType.GetForeignKeys().ToList();
			if (foreignKeys.Any()) {
				result.Add("\tForeign Keys:");
				result.AddRange(foreignKeys.Select(p => $"\t\t{p.ToDebugString()}"));
			}

			var serviceProperties = entityType.GetServiceProperties().ToList();
			if (serviceProperties.Any()) {
				result.Add("\tService Properties:");
				result.AddRange(serviceProperties.Select(p => $"\t\t{p.ToDebugString()}"));
			}

			var skipNavigations = entityType.GetSkipNavigations().ToList();
			if (skipNavigations.Any()) {
				result.Add("\tSkip Navigations:");
				result.AddRange(skipNavigations.Select(p => $"\t\t{p.ToDebugString()}"));
			}

			var sqlMappings = entityType.GetSqlQueryMappings().ToList();
			if (sqlMappings.Any()) {
				result.Add("\tSql Query Navigations:");
				result.AddRange(sqlMappings.Select(p => $"\t\t{p.ToDebugString()}"));
			}
		}

		return string.Join(Environment.NewLine, result);
	}
}