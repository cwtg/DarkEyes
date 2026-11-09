using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;
using DarkEyes.Rules.Dsa5.Characters;
using DarkEyes.Rules.Dsa5.Checks;
using DarkEyes.Rules.Dsa5.Tests.Support;

namespace DarkEyes.Rules.Dsa5.Tests.Unit.Checks;

public sealed class Dsa5ChecksTests
{
    [Fact]
    public void Should_AssessTheCheckAsNotPossible_When_TheLocksModifierLowersAnAttributeToZero()
    {
        // Arrange
        GameState state = new Dsa5Scenario()
            .WithHero(new Attributes(Courage: 12, Sagacity: 12, Intuition: 1, Charisma: 12, Dexterity: 1, Agility: 12, Constitution: 12, Strength: 12), lockpicking: 10)
            .WithLockedChest(checkModifier: -1)
            .BuildState(seed: 1);

        // Act: IN is checked first, so it is reported before FF
        CheckAssessment assessment = new Dsa5Checks().Assess(new CheckRequest(CheckPurposes.PickLock, Dsa5Scenario.Hero, Dsa5Scenario.Chest), state);

        // Assert
        Assert.False(assessment.IsPossible);
        Assert.Equal(Dsa5Reasons.AttributeTooLow(AttributeId.Intuition), assessment.Reason);
    }

    [Fact]
    public void Should_AssessTheCheckAsPossible_When_TheModifiedAttributesStayAboveZero()
    {
        // Arrange
        GameState state = new Dsa5Scenario()
            .WithHero(Dsa5Scenario.AllAttributes(3), lockpicking: 10)
            .WithLockedChest(checkModifier: -2)
            .BuildState(seed: 1);

        // Act
        CheckAssessment assessment = new Dsa5Checks().Assess(new CheckRequest(CheckPurposes.PickLock, Dsa5Scenario.Hero, Dsa5Scenario.Chest), state);

        // Assert
        Assert.True(assessment.IsPossible);
        Assert.Null(assessment.Reason);
    }
}
