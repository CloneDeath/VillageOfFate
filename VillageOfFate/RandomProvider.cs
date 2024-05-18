using System;
using System.Collections.Generic;

namespace VillageOfFate;

public class RandomProvider {
	private readonly Random _random = new();

	public TimeSpan NextTimeSpan(TimeSpan max) => TimeSpan.FromSeconds(_random.NextDouble() * max.TotalSeconds);

	public T SelectOne<T>(IEnumerable<T> items) {
		var list = new List<T>(items);
		return list[_random.Next(list.Count)];
	}
}