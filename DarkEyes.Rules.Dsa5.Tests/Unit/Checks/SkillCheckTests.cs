using DarkEyes.Rules.Dsa5.Checks;
using DarkEyes.Rules.Dsa5.Tests.Support;

namespace DarkEyes.Rules.Dsa5.Tests.Unit.Checks;

public sealed class SkillCheckTests
{
    private static readonly CheckValues s_twelves = new(12, 12, 12);

    [Theory]
    [InlineData(0, 1)]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    [InlineData(6, 2)]
    [InlineData(7, 3)]
    [InlineData(10, 4)]
    [InlineData(13, 5)]
    [InlineData(16, 6)]
    [InlineData(18, 6)]
    public void Should_GiveTheQualityLevel_When_SkillPointsRemain(int skillRating, int expectedQualityLevel)
    {
        // Act: all rolls at or below the attributes, so the whole rating remains
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating, s_twelves, modifier: 0, new FixedRolls(5, 6, 7));

        // Assert
        Assert.Equal(SkillCheckResult.Success, outcome.Result);
        Assert.Equal(expectedQualityLevel, outcome.QualityLevel);
    }

    [Fact]
    public void Should_PayForRollsAboveTheAttributes_FromTheSkillRating()
    {
        // Act: 14 and 13 exceed 12 by 2 and 1, so 3 of 7 points are used and 4 remain
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 7, s_twelves, modifier: 0, new FixedRolls(14, 13, 12));

        // Assert
        Assert.Equal(SkillCheckResult.Success, outcome.Result);
        Assert.Equal(2, outcome.QualityLevel);
    }

    [Fact]
    public void Should_Fail_When_TheSkillRatingCannotPayForTheRolls()
    {
        // Act: 3 points over, only 2 available
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 2, s_twelves, modifier: 0, new FixedRolls(15, 12, 12));

        // Assert
        Assert.Equal(SkillCheckResult.Failure, outcome.Result);
        Assert.Equal(0, outcome.QualityLevel);
    }

    [Fact]
    public void Should_LowerAllThreeAttributes_When_TheModifierIsNegative()
    {
        // Act: succeeds without the modifier (all rolls equal the attributes); -2 costs 2 points per die = 6 > 5
        SkillCheckOutcome plain = SkillCheck.Resolve(skillRating: 5, s_twelves, modifier: 0, new FixedRolls(12, 12, 12));
        SkillCheckOutcome hindered = SkillCheck.Resolve(skillRating: 5, s_twelves, modifier: -2, new FixedRolls(12, 12, 12));

        // Assert
        Assert.Equal(SkillCheckResult.Success, plain.Result);
        Assert.Equal(SkillCheckResult.Failure, hindered.Result);
    }

    [Fact]
    public void Should_RaiseAllThreeAttributes_When_TheModifierIsPositive()
    {
        // Act: 2 points over on each die = 6 > 5 without the modifier; +2 covers all of it
        SkillCheckOutcome plain = SkillCheck.Resolve(skillRating: 5, s_twelves, modifier: 0, new FixedRolls(14, 14, 14));
        SkillCheckOutcome helped = SkillCheck.Resolve(skillRating: 5, s_twelves, modifier: 2, new FixedRolls(14, 14, 14));

        // Assert
        Assert.Equal(SkillCheckResult.Failure, plain.Result);
        Assert.Equal(SkillCheckResult.Success, helped.Result);
    }

    [Fact]
    public void Should_ModifyEachCheckValueOnce_When_AnAttributeIsCheckedTwice()
    {
        // Arrange: IN/FF/FF like lock picking, FF checked twice
        CheckValues intuitionDexterityDexterity = new(14, 12, 12);

        // Act: -1 lowers 14/12/12 to 13/11/11; rolls 13/12/12 then cost exactly 0 + 1 + 1 = 2 of 2 points
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 2, intuitionDexterityDexterity, modifier: -1, new FixedRolls(13, 12, 12));

        // Assert
        Assert.Equal(SkillCheckResult.Success, outcome.Result);
        Assert.Equal(1, outcome.QualityLevel);
    }

    [Fact]
    public void Should_SucceedCritically_When_TwoOnesAreRolled()
    {
        // Act: even with no points to pay for the 20
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 0, new CheckValues(1, 1, 1), modifier: 0, new FixedRolls(1, 20, 1));

        // Assert
        Assert.Equal(SkillCheckResult.CriticalSuccess, outcome.Result);
        Assert.Equal(1, outcome.QualityLevel);
    }

    [Fact]
    public void Should_SucceedSpectacularly_When_ThreeOnesAreRolled()
    {
        // Act
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 0, s_twelves, modifier: 0, new FixedRolls(1, 1, 1));

        // Assert
        Assert.Equal(SkillCheckResult.SpectacularSuccess, outcome.Result);
    }

    [Fact]
    public void Should_Blunder_When_TwoTwentiesAreRolled()
    {
        // Act: even with attributes and rating high enough to pay for everything
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 18, new CheckValues(20, 20, 20), modifier: 0, new FixedRolls(20, 5, 20));

        // Assert
        Assert.Equal(SkillCheckResult.Blunder, outcome.Result);
        Assert.Equal(0, outcome.QualityLevel);
    }

    [Fact]
    public void Should_BlunderSpectacularly_When_ThreeTwentiesAreRolled()
    {
        // Act
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 18, new CheckValues(20, 20, 20), modifier: 0, new FixedRolls(20, 20, 20));

        // Assert
        Assert.Equal(SkillCheckResult.SpectacularBlunder, outcome.Result);
    }

    [Fact]
    public void Should_ComputeTheQualityLevelNormally_When_ACriticalSuccessLeavesPoints()
    {
        // Act: 1, 1, 10 against 12s cost nothing, so all 9 points remain
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 9, s_twelves, modifier: 0, new FixedRolls(1, 1, 10));

        // Assert
        Assert.Equal(SkillCheckResult.CriticalSuccess, outcome.Result);
        Assert.Equal(3, outcome.QualityLevel);
    }

    [Fact]
    public void Should_CapTheQualityLevelAtSix_When_ACriticalSuccessLeavesManyPoints()
    {
        // Act
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 18, s_twelves, modifier: 0, new FixedRolls(1, 1, 5));

        // Assert
        Assert.Equal(SkillCheckResult.CriticalSuccess, outcome.Result);
        Assert.Equal(6, outcome.QualityLevel);
    }

    [Fact]
    public void Should_ReportTheFirstPositionInCheckOrder_When_SeveralValuesDropToZero()
    {
        // Act
        CheckPosition? position = SkillCheck.FirstTooLow(new CheckValues(2, 2, 9), modifier: -2);

        // Assert
        Assert.Equal(CheckPosition.First, position);
    }

    [Fact]
    public void Should_AllowTheCheck_When_AllValuesStayAboveZero()
    {
        // Act
        CheckPosition? position = SkillCheck.FirstTooLow(new CheckValues(3, 3, 3), modifier: -2);

        // Assert
        Assert.Null(position);
    }

    [Fact]
    public void Should_Throw_When_ResolvingACheckThatIsNotPossible()
    {
        // Act: callers check FirstTooLow first, without dice
        static void Act() => SkillCheck.Resolve(skillRating: 5, new CheckValues(2, 12, 12), modifier: -2, new FixedRolls(10, 10, 10));

        // Assert
        Assert.Throws<System.ArgumentException>(Act);
    }

    [Fact]
    public void Should_ReportTheRolls()
    {
        // Act
        SkillCheckOutcome outcome = SkillCheck.Resolve(skillRating: 5, s_twelves, modifier: 0, new FixedRolls(3, 9, 17));

        // Assert
        Assert.Equal(new CheckRolls(3, 9, 17), outcome.Rolls);
    }
}
