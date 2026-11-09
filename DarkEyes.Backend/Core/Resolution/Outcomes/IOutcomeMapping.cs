namespace DarkEyes.Backend.Core.Resolution.Outcomes;

public interface IOutcomeMapping<out TOutcome>
{
    TOutcome Map(in ResolutionResult result);
}
