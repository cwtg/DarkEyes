using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Space;

/// <summary>Where an entity is. Locations and containers as places come later.</summary>
public sealed record Placement(GamePosition Position)
{
    public static ComponentKey<Placement> Key { get; } = new("placement");
}
