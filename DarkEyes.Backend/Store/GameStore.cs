#pragma warning disable CA1822, S2325 // Legacy code from before the architecture; replaced when its use case is ported (architecture §11.2).

using DarkEyes.Backend.Entities.State;
using DarkEyes.Backend.Inventory;
using DarkEyes.Backend.Persistence;
using DarkEyes.Backend.Story;
using DarkEyes.Backend.Time;

namespace DarkEyes.Backend.Store;

public sealed class GameStore
{
    public WorldState World { get; } = new();
    public InventoryPool Inventory { get; } = new();
    public GameTime Time { get; } = new();
    public StoryState Story { get; } = new();

    public LocationState ActiveLocation => World.CurrentArea;

    public SaveDocument Snapshot()
    {
        return new();
    }

    public static GameStore Restore(SaveDocument save)
    {
        return new();
    }

}
