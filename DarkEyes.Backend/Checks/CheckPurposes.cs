namespace DarkEyes.Backend.Checks;

/// <summary>
/// Every purpose the game's acts request checks for: the contract the rule module covers with its mappings
/// (purpose → skill, attributes, modifiers). Acts add their purposes here, not in their own folders.
/// </summary>
public static class CheckPurposes
{
    public static CheckPurpose PickLock { get; } = new("pick_lock");
}
