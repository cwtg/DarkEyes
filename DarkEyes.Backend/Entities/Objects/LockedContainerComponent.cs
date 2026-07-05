namespace DarkEyes.Backend.Entities.Objects
{
    public record LockedContainerComponent(string ComponentId, int ChallengeRating, string ContainerItemDefId, bool Locked) : ILogicComponent
    {
    }
}
