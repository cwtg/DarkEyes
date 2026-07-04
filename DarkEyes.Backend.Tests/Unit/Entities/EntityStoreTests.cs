using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;
using System.Collections.Generic;

namespace DarkEyes.Backend.Tests.Unit.Entities
{
    public sealed class EntityStoreTests
    {
        [Fact]
        public void Should_ExecuteMutations_When_Mutated()
        {
            // Arrange
            EntityId id = new("id");
            EntityStore store = new();
            GamePosition beforePos = new(1, 2, 3);
            GamePosition afterPos = new(0, 0, 0);
            store.Add(new(id, beforePos, new(), null));

            // Act
            var retrievedBefore = store.Get(id);
            store.MutateEntity(id, (e) => e.Position = afterPos);
            var retrievedAfter = store.Get(id);

            // Assert
            Assert.Equal(beforePos, retrievedBefore.Position);
            Assert.Equal(afterPos, retrievedAfter.Position);
        }

        [Fact]
        public void Should_ThrowException_When_RetrievedButNotAdded()
        {
            // Arrange
            EntityId id = new("id");
            EntityStore store = new();

            // Act + Assert
            Assert.Throws<KeyNotFoundException>(() => store.Get(id));
        }
    }
}
