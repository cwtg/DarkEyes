namespace DarkEyes.Backend.Core.Resolution
{
    public interface IResolver
    {
        public ResolutionResult Resolve(ResolutionRequest request);
    }
}
