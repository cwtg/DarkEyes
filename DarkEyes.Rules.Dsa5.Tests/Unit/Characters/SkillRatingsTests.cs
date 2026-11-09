using System;
using DarkEyes.Rules.Dsa5.Characters;

namespace DarkEyes.Rules.Dsa5.Tests.Unit.Characters;

public sealed class SkillRatingsTests
{
    [Fact]
    public void Should_Throw_When_ARatingIsNegative()
    {
        // Act
        static void Act() => SkillRatings.None.With(Skills.Lockpicking.Id, -1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }
}
