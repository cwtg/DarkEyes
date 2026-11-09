using System;

namespace DarkEyes.Rules.Dsa5.Characters;

/// <summary>Identifies one of the eight DSA5 attributes (Eigenschaften); the value is its stable name in reasons and content.</summary>
public readonly record struct AttributeId(string Value)
{
    public static AttributeId Courage { get; } = new("courage");

    public static AttributeId Sagacity { get; } = new("sagacity");

    public static AttributeId Intuition { get; } = new("intuition");

    public static AttributeId Charisma { get; } = new("charisma");

    public static AttributeId Dexterity { get; } = new("dexterity");

    public static AttributeId Agility { get; } = new("agility");

    public static AttributeId Constitution { get; } = new("constitution");

    public static AttributeId Strength { get; } = new("strength");

    public string Value { get; } = string.IsNullOrWhiteSpace(Value)
        ? throw new ArgumentException("An attribute ID must not be empty.", nameof(Value))
        : Value;

    public override string ToString() => Value;
}
