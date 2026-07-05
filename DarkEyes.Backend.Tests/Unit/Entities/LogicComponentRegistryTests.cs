using DarkEyes.Backend.Entities;

namespace DarkEyes.Backend.Tests.Unit.Entities
{
    public sealed class LogicComponentRegistryTests
    {
        [Fact]
        public void Should_ResolveComponent_When_Registered()
        {
            // Arrange
            string id = "comp1";
            LogicComponentRegistry registry = new();

            // Act
            registry.Register(id, new TestLogicComponent(id));
            ILogicComponent retrieved = registry.Resolve(id);

            // Assert
            Assert.Equal(new TestLogicComponent(id).ComponentId, retrieved.ComponentId);
        }

        [Fact]
        public void Should_NotResolveComponent_When_NotRegistered()
        {
            // Arrange
            string id = "comp1";
            LogicComponentRegistry registry = new();

            // Act
            registry.TryResolve(id, out ILogicComponent? retrieved);

            // Assert
            Assert.Null(retrieved);
        }
    }

    sealed file class TestLogicComponent(string id) : ILogicComponent
    {
        public string ComponentId { get; } = id;
    }
}
