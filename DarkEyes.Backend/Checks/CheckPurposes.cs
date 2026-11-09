using System.Collections.Immutable;

namespace DarkEyes.Backend.Checks;

/// <summary>
/// Every purpose the game's acts request checks for: the contract the rule module covers with its mappings
/// (purpose → skill, attributes, modifiers). Acts add their purposes here, not in their own folders.
/// </summary>
public static class CheckPurposes
{
    public static CheckPurpose PickLock { get; } = new("pick_lock");

    /// <summary>Every purpose, for the rule module's completeness test. Add new purposes here too.</summary>
    public static ImmutableArray<CheckPurpose> All { get; } = [PickLock];
}
