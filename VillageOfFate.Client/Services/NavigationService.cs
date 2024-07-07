using System;
using Microsoft.AspNetCore.Components;
using VillageOfFate.WebModels;

namespace VillageOfFate.Client.Services;

public class NavigationService(NavigationManager navigation) {
	public void ToVillager(Guid villagerId) => navigation.NavigateTo($"/villagers/{villagerId}");

	public static string GetUrl(WebItemLocation location) {
		if (location.Villager != null) {
			return $"/villagers/{location.Villager.Id}";
		}

		if (location.Sector != null) {
			return $"/sectors/{location.Sector.Id}";
		}

		throw new Exception("Could not locate the item");
	}
}