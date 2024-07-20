using System;
using Microsoft.AspNetCore.Components;

namespace VillageOfFate.Client.Services;

public class NavigationService(NavigationManager navigation) {
	public void ToVillager(Guid villagerId) => navigation.NavigateTo($"/villagers/{villagerId}");

	public static string GetUrl(Guid itemId) => $"/items/{itemId}";
}