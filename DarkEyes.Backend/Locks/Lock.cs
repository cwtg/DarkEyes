using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Locks;

/// <summary>A lock on an entity (chest, door, …). Everything that has one can be picked.</summary>
public sealed record Lock(bool IsLocked)
{
    public static ComponentKey<Lock> Key { get; } = new("lock");
}
