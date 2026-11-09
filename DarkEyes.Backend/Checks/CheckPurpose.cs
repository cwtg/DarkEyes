using System;

namespace DarkEyes.Backend.Checks;

/// <summary>What a check is for (e.g. picking a lock). The rule module maps it to skill, attributes and modifiers.</summary>
public readonly record struct CheckPurpose(string Value)
{
    public string Value { get; } = string.IsNullOrWhiteSpace(Value)
        ? throw new ArgumentException("A check purpose must not be empty.", nameof(Value))
        : Value;

    public override string ToString() => Value;
}
