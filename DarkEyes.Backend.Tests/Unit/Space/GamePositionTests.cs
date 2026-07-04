using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Tests.Unit.Space
{
    public sealed class GamePositionTests
    {
        [Fact]
        public void Should_IgnoreLevel_When_MeasuringDistanceTo()
        {
            // Arrange
            GamePosition origin = new(0, 1, 2);
            GamePosition target = new(2, 1, 0);

            // Act
            var distance = origin.DistanceTo(target);

            // Assert
            Assert.Equal(2, distance);
        }

        [Fact]
        public void Should_ReturnSixNeighbors()
        {
            // Arrange
            GamePosition origin = new(0, 0, 0);

            // Act
            var neighbors = origin.Neighbors();

            // Arrange
            Assert.Equal(6, neighbors.Count);
        }
    }
}
