using DarkEyes.Backend.Pipeline;
using DarkEyes.Rules.Dsa5.Checks;

namespace DarkEyes.Rules.Dsa5;

/// <summary>The rule module's single registration entry point: installs all DSA5 mechanics.</summary>
public static class Dsa5Rules
{
    public static GameStoreBuilder Install(GameStoreBuilder builder)
    {
        return builder.UseChecks(new Dsa5Checks());
    }
}
