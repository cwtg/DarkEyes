using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Persistence;

namespace DarkEyes.Backend.State
{
    public sealed class GameStore
    {
        public WorldState World { get; } = new();

        public InventoryPool Inventory { get; } = new();

        public GameTime Time { get; } = new();

        public StoryState Story { get; } = new();

        public SaveDocument Snapshot()
        {
            return new();
        }

        public static void Restore(SaveDocument save, LogicComponentRegistry registry)
        {
        }
    }
}
