using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Containers;

public sealed record ContainerState(bool IsOpen)
{
    public static ComponentKey<ContainerState> Key { get; } = new("container");
}
