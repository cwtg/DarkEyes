namespace DarkEyes.Rules.Dsa5.Characters;

/// <summary>The DSA5 skills the game uses so far.</summary>
public static class Skills
{
    /// <summary>Schloesserknacken: IN/FF/FF.</summary>
    public static SkillDefinition Lockpicking { get; } = new(new SkillId("skill.lockpicking"), AttributeId.Intuition, AttributeId.Dexterity, AttributeId.Dexterity);
}
