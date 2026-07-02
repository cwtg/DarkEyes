using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Core;

namespace DarkEyes.Backend.Space.Events
{
    public record CharacterSteppedEvent(EntityId Id, GamePosition From, GamePosition To) : IGameEvent
    {
    }
}
