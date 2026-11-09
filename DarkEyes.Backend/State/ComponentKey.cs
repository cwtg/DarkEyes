using System;

namespace DarkEyes.Backend.State;

/// <summary>The stable name under which a component is stored. Not bound 1:1 to a type.</summary>
public abstract record ComponentKey
{
    private protected ComponentKey(string name)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("A component name must not be empty.", nameof(name))
            : name;
    }

    public string Name { get; }

    /// <summary>Whether a value may be stored under this key; guards the untyped write path.</summary>
    public abstract bool Accepts(object component);

    public override string ToString() => Name;
}
