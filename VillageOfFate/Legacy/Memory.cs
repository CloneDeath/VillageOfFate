using System.Collections;
using System.Collections.Generic;

namespace VillageOfFate.Legacy;

public class Memory : IEnumerable<string> {
	private readonly List<string> _history = [];

	public IEnumerator<string> GetEnumerator() => _history.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(string message) {
		_history.Add(message);
	}
}