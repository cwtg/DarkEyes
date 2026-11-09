using System.Collections.Generic;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>A check service that rolls d20s from the rules stream and records them; every check succeeds.</summary>
internal sealed class RollingChecks(int rollsPerCheck = 1) : IChecks
{
    private readonly int _rollsPerCheck = rollsPerCheck;
    private readonly List<int> _rolls = [];

    public IReadOnlyList<int> Rolls => _rolls;

    public CheckAssessment Assess(CheckRequest request, GameState state) => CheckAssessment.Possible;

    public CheckResult Roll(CheckRequest request, CheckContext context)
    {
        for (int i = 0; i < _rollsPerCheck; i++)
        {
            _rolls.Add(context.Random.Next(1, 21));
        }

        return new CheckSucceeded(degree: 1);
    }
}
