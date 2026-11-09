using System;
using DarkEyes.Backend.Checks;

namespace DarkEyes.Backend.Tests.Unit.Checks;

public sealed class CheckResultTests
{
    private static readonly Reason s_tooHard = Reason.Create("rules.too_hard");

    [Fact]
    public void Should_TreatNotPossibleAsFailed_When_TheCallerDoesNotHandleIt()
    {
        // Act
        string outcome = new CheckNotPossible(s_tooHard).Match(succeeded: _ => "succeeded", failed: () => "failed");

        // Assert
        Assert.Equal("failed", outcome);
    }

    [Fact]
    public void Should_PassTheReason_When_TheCallerHandlesNotPossible()
    {
        // Act
        Reason? reported = new CheckNotPossible(s_tooHard).Match<Reason?>(succeeded: _ => null, failed: () => null, notPossible: reason => reason);

        // Assert
        Assert.Equal(s_tooHard, reported);
    }

    [Fact]
    public void Should_PassTheDegree_When_TheCheckSucceeded()
    {
        // Act
        int degree = new CheckSucceeded(degree: 3).Match(succeeded: d => d, failed: () => 0);

        // Assert
        Assert.Equal(3, degree);
    }

    [Fact]
    public void Should_Throw_When_ASuccessHasNoDegree()
    {
        // Act
        static void Act() => _ = new CheckSucceeded(degree: 0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }
}
