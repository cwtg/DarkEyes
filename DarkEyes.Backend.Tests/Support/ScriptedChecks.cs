using System.Collections.Generic;
using DarkEyes.Backend.Checks;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>Stands in for the rule module's check service: every check has the scripted result; requests are recorded.</summary>
internal sealed class ScriptedChecks(bool succeed) : IChecks
{
    private readonly bool _succeed = succeed;
    private readonly List<CheckRequest> _requests = [];

    public IReadOnlyList<CheckRequest> Requests => _requests;

    public CheckResult Roll(CheckRequest request, CheckContext context)
    {
        _requests.Add(request);
        return new CheckResult(_succeed, Degree: _succeed ? 1 : 0);
    }
}
