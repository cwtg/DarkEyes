namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>The three attribute values a skill check is rolled against, in the skill's order.</summary>
public readonly record struct CheckValues(int First, int Second, int Third);
