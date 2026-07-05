using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Events
{
    public record CharacterSteppedEvent(EntityId Id, GamePosition From, GamePosition To, CellContext CellContext) : IGameEvent
    {
    }
}
