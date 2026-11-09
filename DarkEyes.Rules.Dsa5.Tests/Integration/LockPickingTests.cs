using System.Linq;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.LockPicking;
using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Rules.Dsa5.Checks;
using DarkEyes.Rules.Dsa5.Characters;
using DarkEyes.Rules.Dsa5.Tests.Support;

namespace DarkEyes.Rules.Dsa5.Tests.Integration;

public sealed class LockPickingTests
{
    [Fact]
    public void Should_RollLockpickingAndUnlock_When_AMasterThiefPicksTheLock()
    {
        // Arrange: attributes 20 and rating 18 pay for any roll, so only a blunder could fail (not with this seed)
        GameStore game = new Dsa5Scenario()
            .WithHero(Dsa5Scenario.AllAttributes(20), lockpicking: 18)
            .WithLockedChest(checkModifier: 0)
            .Build(seed: 1);

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest));

        // Assert
        CommitResult commit = Assert.IsType<Committed>(result).Commit;
        SkillCheckRolled check = Assert.IsType<SkillCheckRolled>(commit.Steps[0].Event);
        Assert.Equal(Skills.Lockpicking.Id, check.Skill);
        Assert.Equal(CheckPurposes.PickLock, check.Purpose);
        Assert.Equal(Dsa5Scenario.Chest, check.Target);
        Assert.True(check.QualityLevel >= 1);
        Assert.Equal(new LockUnlocked(Dsa5Scenario.Chest, Dsa5Scenario.Hero), commit.Steps[1].Event);
    }

    [Fact]
    public void Should_ApplyTheLocksModifier_When_PickingIt()
    {
        // Arrange
        GameStore game = new Dsa5Scenario()
            .WithHero(Dsa5Scenario.AllAttributes(12), lockpicking: 6)
            .WithLockedChest(checkModifier: -3)
            .Build(seed: 1);

        // Act
        CommitResult commit = Assert.IsType<Committed>(game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert
        Assert.Equal(-3, Assert.IsType<SkillCheckRolled>(commit.Steps[0].Event).Modifier);
    }

    [Fact]
    public void Should_RollAgainstIntuitionAndDexterityTwice_When_PickingALock()
    {
        // Arrange: distinct values, so the order IN/FF/FF is visible
        GameStore game = new Dsa5Scenario()
            .WithHero(new Attributes(Courage: 11, Sagacity: 12, Intuition: 14, Charisma: 13, Dexterity: 10, Agility: 9, Constitution: 8, Strength: 7), lockpicking: 4)
            .WithLockedChest(checkModifier: 0)
            .Build(seed: 1);

        // Act
        CommitResult commit = Assert.IsType<Committed>(game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert
        Assert.Equal(new CheckValues(14, 10, 10), Assert.IsType<SkillCheckRolled>(commit.Steps[0].Event).Values);
    }

    [Fact]
    public void Should_ReportNotPossibleWithoutRolling_When_TheLocksModifierLowersAnAttributeToZero()
    {
        // Arrange: dexterity 2 with a -2 lock leaves FF at 0, so the check can't be attempted (DSA5)
        GameStore game = new Dsa5Scenario()
            .WithHero(new Attributes(Courage: 12, Sagacity: 12, Intuition: 12, Charisma: 12, Dexterity: 2, Agility: 12, Constitution: 12, Strength: 12), lockpicking: 10)
            .WithLockedChest(checkModifier: -2)
            .Build(seed: 1);

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest));

        // Assert: an outcome, not a rejection, and no SkillCheckRolled (no dice)
        CommitResult commit = Assert.IsType<Committed>(result).Commit;
        LockPickNotPossible notPossible = Assert.IsType<LockPickNotPossible>(Assert.Single(commit.Steps).Event);
        Assert.Equal(Dsa5Reasons.AttributeTooLow(AttributeId.Dexterity), notPossible.Reason);
    }

    [Fact]
    public void Should_AllowTheCheck_When_TheModifiedAttributesStayAboveZero()
    {
        // Arrange: dexterity 3 with a -2 lock leaves FF at 1
        GameStore game = new Dsa5Scenario()
            .WithHero(new Attributes(Courage: 12, Sagacity: 12, Intuition: 12, Charisma: 12, Dexterity: 3, Agility: 12, Constitution: 12, Strength: 12), lockpicking: 10)
            .WithLockedChest(checkModifier: -2)
            .Build(seed: 1);

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest));

        // Assert: a "not possible" outcome would commit too, so check that dice were rolled
        Assert.IsType<SkillCheckRolled>(Assert.IsType<Committed>(result).Commit.Steps[0].Event);
    }

    [Fact]
    public void Should_RollNoDice_When_TheCheckIsNotPossible()
    {
        // Arrange: the hard chest is impossible for this hero, the easy one possible
        Dsa5Scenario scenario = new Dsa5Scenario()
            .WithHero(new Attributes(Courage: 12, Sagacity: 12, Intuition: 12, Charisma: 12, Dexterity: 2, Agility: 12, Constitution: 12, Strength: 12), lockpicking: 10)
            .WithLockedChest(checkModifier: 0)
            .WithLockedChest(Dsa5Scenario.HardChest, checkModifier: -2);
        GameStore triedTheHardOneFirst = scenario.Build(seed: 3);
        GameStore pickedTheEasyOneOnly = scenario.Build(seed: 3);

        // Act
        triedTheHardOneFirst.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.HardChest));
        CommitResult afterTheImpossibleOne = Assert.IsType<Committed>(triedTheHardOneFirst.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;
        CommitResult directly = Assert.IsType<Committed>(pickedTheEasyOneOnly.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert: the impossible attempt consumed nothing from the rules stream
        Assert.Equal(Assert.IsType<SkillCheckRolled>(directly.Steps[0].Event).Rolls, Assert.IsType<SkillCheckRolled>(afterTheImpossibleOne.Steps[0].Event).Rolls);
    }

    [Fact]
    public void Should_NotUnlock_When_TheCheckFails()
    {
        // Arrange: attributes 1 and rating 0 fail every roll except a critical success (not with this seed)
        GameStore game = new Dsa5Scenario()
            .WithHero(Dsa5Scenario.AllAttributes(1), lockpicking: 0)
            .WithLockedChest(checkModifier: 0)
            .Build(seed: 1);

        // Act
        CommitResult commit = Assert.IsType<Committed>(game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert
        SkillCheckRolled check = Assert.IsType<SkillCheckRolled>(Assert.Single(commit.Steps).Event);
        Assert.False(check.Result is SkillCheckResult.Success or SkillCheckResult.CriticalSuccess or SkillCheckResult.SpectacularSuccess);
    }

    [Fact]
    public void Should_RollTheSameCheck_When_SeedAndSituationAreTheSame()
    {
        // Arrange
        Dsa5Scenario scenario = new Dsa5Scenario().WithHero(Dsa5Scenario.AllAttributes(12), lockpicking: 6).WithLockedChest(checkModifier: -1);

        // Act
        CommitResult first = Assert.IsType<Committed>(scenario.Build(seed: 7).Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;
        CommitResult second = Assert.IsType<Committed>(scenario.Build(seed: 7).Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert
        Assert.Equal(first.Steps.Select(step => step.Event), second.Steps.Select(step => step.Event));
    }

    [Fact]
    public void Should_RollDifferently_When_SeedsDiffer()
    {
        // Arrange
        Dsa5Scenario scenario = new Dsa5Scenario().WithHero(Dsa5Scenario.AllAttributes(12), lockpicking: 6).WithLockedChest(checkModifier: 0);

        // Act
        CommitResult first = Assert.IsType<Committed>(scenario.Build(seed: 7).Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;
        CommitResult second = Assert.IsType<Committed>(scenario.Build(seed: 8).Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert
        Assert.NotEqual(Assert.IsType<SkillCheckRolled>(first.Steps[0].Event).Rolls, Assert.IsType<SkillCheckRolled>(second.Steps[0].Event).Rolls);
    }

    [Fact]
    public void Should_UseRatingZeroAndNoModifier_When_TheHeroHasNoRatingsAndTheLockNoModifier()
    {
        // Arrange
        GameStore game = new Dsa5Scenario()
            .WithHeroWithoutRatings(Dsa5Scenario.AllAttributes(12))
            .WithLockedChestWithoutModifier()
            .Build(seed: 1);

        // Act
        CommitResult commit = Assert.IsType<Committed>(game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest))).Commit;

        // Assert
        SkillCheckRolled check = Assert.IsType<SkillCheckRolled>(commit.Steps[0].Event);
        Assert.Equal(0, check.Modifier);
        Assert.Equal(new CheckValues(12, 12, 12), check.Values);
    }

    [Fact]
    public void Should_Throw_When_TheActorHasNoAttributes()
    {
        // Arrange: a content bug (a hero without DSA attributes), not a rule outcome
        GameStore game = new Dsa5Scenario().WithHeroWithoutAttributes().WithLockedChest(checkModifier: 0).Build(seed: 1);

        // Act
        void Act() => game.Dispatch(new PickLockAction(Dsa5Scenario.Hero, Dsa5Scenario.Chest));

        // Assert
        Assert.Throws<System.Collections.Generic.KeyNotFoundException>(Act);
    }

}
