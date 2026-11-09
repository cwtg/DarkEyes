using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Space;

/// <summary>Distances between placed entities. How far an act may reach is up to the act, not to this class.</summary>
public static class PlacementDistance
{
    /// <summary>Hex distance between the placements of two entities. Ignores the level (known gap until levels are ported); becomes footprint-aware with movement.</summary>
    public static int Between(GameState state, EntityId from, EntityId to)
    {
        GamePosition fromPosition = state.GetEntity(from).Get(Placement.Key).Position;
        GamePosition toPosition = state.GetEntity(to).Get(Placement.Key).Position;
        return fromPosition.DistanceTo(toPosition);
    }
}
