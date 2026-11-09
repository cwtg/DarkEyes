using System;
using DarkEyes.Backend;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;
using DarkEyes.Rules.Dsa5.Characters;

namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>The backend's check service, implemented with the DSA5 skill check.</summary>
internal sealed class Dsa5Checks : IChecks
{
    public CheckAssessment Assess(CheckRequest request, GameState state)
    {
        PreparedCheck check = Prepare(request, state);
        return check.NotPossibleBecause is { } reason ? CheckAssessment.Impossible(reason) : CheckAssessment.Possible;
    }

    public CheckResult Roll(CheckRequest request, CheckContext context)
    {
        PreparedCheck check = Prepare(request, context.State);
        if (check.NotPossibleBecause is { } reason)
        {
            return new CheckNotPossible(reason);
        }

        SkillCheckOutcome outcome = SkillCheck.Resolve(check.Rating, check.Values, check.Modifier, context.Random);
        context.Emit(new SkillCheckRolled(request.Actor, request.Purpose, request.Target, check.Skill.Id, check.Values, check.Modifier, outcome.Rolls, outcome.Result, outcome.QualityLevel));
        return outcome.Succeeded ? new CheckSucceeded(outcome.QualityLevel) : new CheckFailed();
    }

    private static PreparedCheck Prepare(CheckRequest request, GameState state)
    {
        if (!PurposeMappings.TryGet(request.Purpose, out PurposeMapping mapping))
        {
            throw new InvalidOperationException($"The DSA5 rules have no mapping for check purpose '{request.Purpose}'.");
        }

        Entity actor = state.GetEntity(request.Actor);
        Attributes attributes = actor.Get(Attributes.Key);
        int rating = actor.Has(SkillRatings.Key) ? actor.Get(SkillRatings.Key).Of(mapping.Skill.Id) : 0;
        int modifier = mapping.Modifier(request, state);
        CheckValues values = new(attributes.Of(mapping.Skill.First), attributes.Of(mapping.Skill.Second), attributes.Of(mapping.Skill.Third));

        Reason? notPossibleBecause = SkillCheck.FirstTooLow(values, modifier) is { } position
            ? Dsa5Reasons.AttributeTooLow(AttributeAt(mapping.Skill, position))
            : null;

        return new PreparedCheck(mapping.Skill, rating, values, modifier, notPossibleBecause);
    }

    private static AttributeId AttributeAt(SkillDefinition skill, CheckPosition position) => position switch
    {
        CheckPosition.First => skill.First,
        CheckPosition.Second => skill.Second,
        CheckPosition.Third => skill.Third,
        _ => throw new ArgumentOutOfRangeException(nameof(position), position, "Unknown check position."),
    };

    private sealed record PreparedCheck(SkillDefinition Skill, int Rating, CheckValues Values, int Modifier, Reason? NotPossibleBecause);
}
