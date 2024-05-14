using System.Collections;
using System.Collections.Generic;

namespace VillageOfFate;

public class Memory : IEnumerable<string> {
	private readonly List<string> _history = [];

	public void Add(string message) {
		_history.Add(message);
	}

	public IEnumerator<string> GetEnumerator() => _history.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}