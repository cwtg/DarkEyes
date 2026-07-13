using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Handlers;

namespace DarkEyes.Backend.Commands
{
    public sealed class OpenContainerCommand(EntityId containerId, EntityId actorId) : ICommand
    {
        public EntityId ContainerId { get; } = containerId;
        public EntityId ActorId { get; } = actorId;
    }
}
