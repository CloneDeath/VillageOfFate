﻿using System;
using System.Collections.Generic;
using VillageOfFate.WebModels.Activities;

namespace VillageOfFate.WebModels;

public class WebVillager {
	public Guid Id { get; init; } = Guid.Empty;
	public string Name { get; init; } = "Villager";
	public int Age { get; init; } = 18;
	public string Summary { get; init; } = string.Empty;
	public Gender Gender { get; init; } = Gender.Male;
	public WebVillagerEmotions Emotions { get; init; } = new();
	public required Position SectorLocation { get; init; }
	public required int Hunger { get; init; }
	public required Guid ImageId { get; init; }
	public WebActivity? CurrentActivity { get; init; }
	public Stack<WebActivity> ActivityQueue { get; init; } = new();
	public List<WebItem> Inventory { get; init; } = [];
}