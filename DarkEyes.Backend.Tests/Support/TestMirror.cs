using System;
using System.Collections.Generic;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Visibility;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>What the frontend would see: a copy of the projection, updated only through patches.</summary>
internal sealed class TestMirror
{
    private readonly Dictionary<EntityId, Dictionary<string, object>> _entities = [];

    private TestMirror()
    {
    }

    public static TestMirror Connect(GameStore store)
    {
        TestMirror mirror = new();
        mirror.Apply(store.Connect());
        return mirror;
    }

    public void Apply(CommitResult commit)
    {
        foreach (Step step in commit.Steps)
        {
            Apply(step.Patches);
        }
    }

    public T Get<T>(EntityId id, ComponentKey<T> key)
        where T : class
    {
        return (T)_entities[id][key.Name];
    }

    private void Apply(IEnumerable<Patch> patches)
    {
        foreach (Patch patch in patches)
        {
            switch (patch)
            {
                case EntityAppeared appeared:
                    _entities.Add(appeared.Entity, new Dictionary<string, object>(appeared.Components, StringComparer.Ordinal));
                    break;
                case ComponentReplaced replaced:
                    _entities[replaced.Entity][replaced.Component] = replaced.Value;
                    break;
                default:
                    throw new NotSupportedException($"The test mirror doesn't support {patch.GetType().Name} yet.");
            }
        }
    }
}
