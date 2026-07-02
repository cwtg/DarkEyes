using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Entities;

namespace DarkEyes.Backend.Space.Events
{
    public record CharacterSteppedEvent(EntityId Id, GamePosition From, GamePosition To) : IGameEvent
    {
    }
}
