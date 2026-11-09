using System.Collections.Generic;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>Stands in for the rule module's check service: every check has the scripted result; rolled requests are recorded.</summary>
internal sealed class ScriptedChecks(CheckResult result) : IChecks
{
    private readonly CheckResult _result = result;
    private readonly List<CheckRequest> _requests = [];

    public ScriptedChecks(bool succeed)
        : this(succeed ? new CheckSucceeded(degree: 1) : new CheckFailed())
    {
    }

    public IReadOnlyList<CheckRequest> Requests => _requests;

    public CheckAssessment Assess(CheckRequest request, GameState state) =>
        _result is CheckNotPossible notPossible ? CheckAssessment.Impossible(notPossible.Reason) : CheckAssessment.Possible;

    public CheckResult Roll(CheckRequest request, CheckContext context)
    {
        _requests.Add(request);
        return _result;
    }
}
