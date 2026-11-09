using DarkEyes.Backend.Entities;

namespace DarkEyes.Backend.Handlers;

public sealed class OpenContainerCommand(EntityId containerId, EntityId actorId) : ICommand
{
    public EntityId ContainerId { get; } = containerId;
    public EntityId ActorId { get; } = actorId;
}
