using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Checks;

/// <summary>The check service, supplied by the rule module.</summary>
public interface IChecks
{
    /// <summary>Side-effect free, without dice: may the check be attempted? For previews and AI look-ahead, never in validation.</summary>
    CheckAssessment Assess(CheckRequest request, GameState state);

    CheckResult Roll(CheckRequest request, CheckContext context);
}
