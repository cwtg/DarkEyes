using System.Collections.Generic;

namespace DarkEyes.Backend.Entities.State
{
    public record LockedContainerState(string ComponentId, int ChallengeRating, IReadOnlyList<string> ContainedItemDefIds, bool Locked)
    {
        public LockedContainerState Unlock() => this with { Locked = false };
    }
}
