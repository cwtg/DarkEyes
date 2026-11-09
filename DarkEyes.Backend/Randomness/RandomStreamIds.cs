namespace DarkEyes.Backend.Randomness;

/// <summary>The streams of the game's random consumers.</summary>
public static class RandomStreamIds
{
    /// <summary>Rule resolution: checks, damage, everything the rules roll inside chains.</summary>
    public static RandomStreamId Rules { get; } = new("rules");
}
