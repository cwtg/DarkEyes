namespace DarkEyes.Rules.Dsa5.Characters;

/// <summary>Rule data of a skill: the three attributes it is checked against, in order.</summary>
public sealed record SkillDefinition(SkillId Id, AttributeId First, AttributeId Second, AttributeId Third);
