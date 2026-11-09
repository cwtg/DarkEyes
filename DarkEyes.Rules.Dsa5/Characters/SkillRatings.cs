using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DarkEyes.Backend.State;

namespace DarkEyes.Rules.Dsa5.Characters;

/// <summary>A character's skill ratings (Fertigkeitswerte); a skill without an entry has rating 0.</summary>
public sealed record SkillRatings
{
    private SkillRatings(ImmutableSortedDictionary<SkillId, int> ratings)
    {
        Ratings = ratings;
    }

    public static ComponentKey<SkillRatings> Key { get; } = new("dsa5.skill_ratings");

    public static SkillRatings None { get; } = new(ImmutableSortedDictionary.Create<SkillId, int>(SkillId.Ordinal));

    public ImmutableSortedDictionary<SkillId, int> Ratings { get; }

    public int Of(SkillId skill) => Ratings.TryGetValue(skill, out int rating) ? rating : 0;

    public SkillRatings With(SkillId skill, int rating) => rating >= 0
        ? new(Ratings.SetItem(skill, rating))
        : throw new ArgumentOutOfRangeException(nameof(rating), rating, "A skill rating can't be negative.");

    /// <summary>Value equality over the ratings (a record alone would compare the map by reference).</summary>
    public bool Equals(SkillRatings? other) => other is not null && Ratings.SequenceEqual(other.Ratings);

    public override int GetHashCode()
    {
        HashCode hash = default;
        foreach (KeyValuePair<SkillId, int> rating in Ratings)
        {
            hash.Add(rating.Key);
            hash.Add(rating.Value);
        }

        return hash.ToHashCode();
    }
}
