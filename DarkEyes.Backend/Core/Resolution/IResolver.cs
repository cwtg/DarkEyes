namespace DarkEyes.Backend.Core.Resolution;

public interface IResolver
{
    ResolutionResult Resolve(ResolutionRequest request, IRandomSource random);
}
