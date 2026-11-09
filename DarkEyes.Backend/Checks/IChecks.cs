namespace DarkEyes.Backend.Checks;

/// <summary>The check service, supplied by the rule module.</summary>
public interface IChecks
{
    CheckResult Roll(CheckRequest request, CheckContext context);
}
