using System;
using System.Collections.Generic;

namespace DarkEyes.Rules.Dsa5.Characters;

/// <summary>Identifies a skill (content ID, e.g. <c>skill.lockpicking</c>).</summary>
public readonly record struct SkillId(string Value)
{
    public static IComparer<SkillId> Ordinal { get; } = Comparer<SkillId>.Create((left, right) => string.CompareOrdinal(left.Value, right.Value));

    public string Value { get; } = string.IsNullOrWhiteSpace(Value)
        ? throw new ArgumentException("A skill ID must not be empty.", nameof(Value))
        : Value;

    public override string ToString() => Value;
}
