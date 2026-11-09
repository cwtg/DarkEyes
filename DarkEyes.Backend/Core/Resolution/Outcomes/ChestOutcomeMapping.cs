namespace DarkEyes.Backend.Core.Resolution.Outcomes;

public sealed class ChestOutcomeMapping : IOutcomeMapping<bool>
{
    public bool Map(in ResolutionResult result) => result.IsSuccess;
}
