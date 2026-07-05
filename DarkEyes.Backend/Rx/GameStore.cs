using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Persistence;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Rx
{
    public sealed class GameStore
    {
        public WorldState World { get; } = new();

        public InventoryPool Inventory { get; } = new();

        public GameTime Time { get; } = new();

        public StoryState Story { get; } = new();

        public SaveDocument Snaphot()
        {
            return new();
        }

        public static void Restore(SaveDocument save, LogicComponentRegistry registry)
        {
        }
    }
}
