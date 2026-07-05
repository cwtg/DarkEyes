using System.Collections.Generic;

namespace DarkEyes.Backend.Entities.Components
{
    public record LockedContainerComponent(string ComponentId, int ChallengeRating, IReadOnlyList<string> ContainedItemDefIds, bool Locked) : ILogicComponent
    {
    }
}
