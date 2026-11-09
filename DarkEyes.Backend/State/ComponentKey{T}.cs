namespace DarkEyes.Backend.State;

/// <summary>Typed access to the component stored under <see cref="ComponentKey.Name"/>.</summary>
public sealed record ComponentKey<T> : ComponentKey
    where T : class
{
    public ComponentKey(string name)
        : base(name)
    {
    }

    public override bool Accepts(object component) => component is T;
}
