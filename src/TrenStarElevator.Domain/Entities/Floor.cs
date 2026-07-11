namespace TrenStarElevator.Domain.Entities;

public sealed class Floor : IEquatable<Floor>
{
    public int Number { get; }
    public string Name { get; }

    public Floor(int number, string? name = null)
    {
        Number = number;
        Name = name ?? (number == 0 ? "Ground Floor" : $"Floor {number}");
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj) => obj is Floor other && Equals(other);
    
    public bool Equals(Floor? other) => other is not null && other.Number == Number;

    public override int GetHashCode() => Number.GetHashCode();

    public static bool operator ==(Floor? left, Floor? right) => Equals(left, right);

    public static bool operator !=(Floor? left, Floor? right) => !Equals(left, right);
}