using System;
using System.Collections.Generic;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Tests.Unit.Entities;

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
        Entity retrievedBefore = store.Get(id);
        store.MutateEntity(id, (e) => { e.Position = afterPos; return e; });
        Entity retrievedAfter = store.Get(id);

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

    [Fact]
    public void Should_ReplaceStateInstance_When_Mutated()
    {
        // Arrange
        EntityId id = new("id");
        EntityStore store = new();
        Dictionary<Type, object> initialStates = new() { { typeof(SampleState), new SampleState(Locked: true) } };
        store.Add(new(id, new(), new(), [], initialStates));

        // Act
        Entity retrievedBefore = store.Get(id);
        retrievedBefore.States.TryGetValue(typeof(SampleState), out object? stateBefore);
        var before = stateBefore! as SampleState;
        store.MutateEntity(id, (e) => e.WithState(before!.Unlock()));
        Entity retrievedAfter = store.Get(id);
        retrievedAfter.States.TryGetValue(typeof(SampleState), out object? stateAfter);
        var after = stateAfter! as SampleState;

        // Assert
        Assert.True(before!.Locked);
        Assert.False(after!.Locked);
    }
}

/// <summary>An example state for the old store (the state type it used before was deleted with the old chest code).</summary>
file sealed record SampleState(bool Locked)
{
    public SampleState Unlock() => this with { Locked = false };
}
