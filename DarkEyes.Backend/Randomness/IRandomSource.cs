namespace DarkEyes.Backend.Randomness;

/// <summary>Random numbers from one stream of the game state.</summary>
public interface IRandomSource
{
    /// <summary>A uniformly distributed value in [minInclusive, maxExclusive).</summary>
    int Next(int minInclusive, int maxExclusive);
}
