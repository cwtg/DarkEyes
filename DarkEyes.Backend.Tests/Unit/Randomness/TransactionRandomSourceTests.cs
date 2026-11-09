using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Randomness;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Visibility;

namespace DarkEyes.Backend.Tests.Unit.Randomness;

public sealed class TransactionRandomSourceTests
{
    [Theory]
    [InlineData(-5, 5)]
    [InlineData(int.MinValue, int.MaxValue)]
    [InlineData(int.MaxValue - 1, int.MaxValue)]
    public void Should_StayWithinTheRange_When_TheRangeIsNegativeOrExtreme(int minInclusive, int maxExclusive)
    {
        // Arrange
        Transaction transaction = new(GameState.Empty.WithMasterSeed(1), Projection.Empty);
        TransactionRandomSource random = new(transaction, RandomStreamIds.Rules);

        // Act + Assert
        for (int i = 0; i < 200; i++)
        {
            int value = random.Next(minInclusive, maxExclusive);
            Assert.InRange(value, minInclusive, maxExclusive - 1);
        }
    }
}
