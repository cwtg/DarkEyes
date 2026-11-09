using DarkEyes.Backend.Entities.State;

namespace DarkEyes.Backend.Store;

public class WorldState
{
    public LocationState CurrentArea { get; private set; } = new();
}
