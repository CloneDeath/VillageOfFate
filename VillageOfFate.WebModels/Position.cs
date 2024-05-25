using System;

namespace VillageOfFate.WebModels;

public readonly struct Position(int x, int y) : IEquatable<Position> {
	public Position() : this(0, 0) { }

	public int X { get; init; } = x;
	public int Y { get; init; } = y;

	public static readonly Position Zero = new();

	#region IEquatable<Position>
	public bool Equals(Position other) => X == other.X && Y == other.Y;
	public override bool Equals(object? obj) => obj is Position other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(X, Y);
	public static bool operator ==(Position left, Position right) => left.Equals(right);
	public static bool operator !=(Position left, Position right) => !left.Equals(right);
	#endregion
}