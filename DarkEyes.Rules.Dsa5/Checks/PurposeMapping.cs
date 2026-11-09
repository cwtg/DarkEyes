using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;
using DarkEyes.Rules.Dsa5.Characters;

namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>How the rules check one purpose: which skill, and which modifier the situation adds.</summary>
internal sealed record PurposeMapping(SkillDefinition Skill, Func<CheckRequest, GameState, int> Modifier);
