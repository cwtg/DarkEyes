using DarkEyes.Backend.Checks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;
using DarkEyes.Rules.Dsa5.Characters;

namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>A skill check was rolled: for what, against which attribute values (before the modifier), with which modifier, and how it went.</summary>
public sealed record SkillCheckRolled(
    EntityId Actor,
    CheckPurpose Purpose,
    EntityId? Target,
    SkillId Skill,
    CheckValues Values,
    int Modifier,
    CheckRolls Rolls,
    SkillCheckResult Result,
    int QualityLevel) : IEvent;
