using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VillageOfFate.Legacy;

public class RelationshipMemory : IEnumerable<Relationship> {
	private readonly Dictionary<Villager, string> _relationship = new();

	public IEnumerator<Relationship> GetEnumerator() =>
		_relationship.Select(kvp => new Relationship(kvp.Key, kvp.Value)).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(Villager villager, string relationship) {
		_relationship[villager] = relationship;
	}
}