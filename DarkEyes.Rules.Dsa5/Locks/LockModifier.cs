using DarkEyes.Backend.State;

namespace DarkEyes.Rules.Dsa5.Locks;

/// <summary>A lock's modifier to lock-picking checks, signed as in DSA notation: -2 is a penalty of 2, +1 a bonus.</summary>
public sealed record LockModifier(int CheckModifier)
{
    public static ComponentKey<LockModifier> Key { get; } = new("dsa5.lock_modifier");
}
