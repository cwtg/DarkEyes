using System;
using DarkEyes.Backend.State;

namespace DarkEyes.Rules.Dsa5.Characters;

/// <summary>A character's attribute values.</summary>
public sealed record Attributes(int Courage, int Sagacity, int Intuition, int Charisma, int Dexterity, int Agility, int Constitution, int Strength)
{
    public static ComponentKey<Attributes> Key { get; } = new("dsa5.attributes");

    public int Of(AttributeId attribute)
    {
        if (attribute == AttributeId.Courage)
        {
            return Courage;
        }

        if (attribute == AttributeId.Sagacity)
        {
            return Sagacity;
        }

        if (attribute == AttributeId.Intuition)
        {
            return Intuition;
        }

        if (attribute == AttributeId.Charisma)
        {
            return Charisma;
        }

        if (attribute == AttributeId.Dexterity)
        {
            return Dexterity;
        }

        if (attribute == AttributeId.Agility)
        {
            return Agility;
        }

        if (attribute == AttributeId.Constitution)
        {
            return Constitution;
        }

        if (attribute == AttributeId.Strength)
        {
            return Strength;
        }

        throw new ArgumentOutOfRangeException(nameof(attribute), attribute, "Unknown attribute.");
    }
}
