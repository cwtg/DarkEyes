namespace DarkEyes.Backend.Core.Resolution.Outcomes
{
    public interface IOutcomeMapping<out TOutcome>
    {
        public TOutcome Map(in ResolutionResult result);
    }
}
