using System;
using System.Collections.Immutable;

namespace DarkEyes.Backend.State;

/// <summary>An entity: identity plus components, keyed by stable name. Only created and changed through its own methods.</summary>
public sealed record Entity
{
    private Entity(EntityId id, ImmutableSortedDictionary<string, object> components)
    {
        Id = id;
        Components = components;
    }

    public EntityId Id { get; }

    internal ImmutableSortedDictionary<string, object> Components { get; private init; }

    public static Entity Create(EntityId id)
    {
        if (id == default)
        {
            throw new ArgumentException("An entity needs an ID.", nameof(id));
        }

        return new(id, ImmutableSortedDictionary.Create<string, object>(StringComparer.Ordinal));
    }

    public T Get<T>(ComponentKey<T> key)
        where T : class
    {
        return (T)Components[key.Name];
    }

    public Entity With<T>(ComponentKey<T> key, T component)
        where T : class
    {
        return With((ComponentKey)key, component);
    }

    internal Entity With(ComponentKey key, object component)
    {
        if (!key.Accepts(component))
        {
            throw new ArgumentException($"A {component.GetType().Name} can't be stored as component '{key.Name}'.", nameof(component));
        }

        return this with { Components = Components.SetItem(key.Name, component) };
    }
}
