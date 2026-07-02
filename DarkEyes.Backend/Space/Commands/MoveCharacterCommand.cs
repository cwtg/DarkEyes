using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Core;

namespace DarkEyes.Backend.Space.Commands
{
    public record MoveCharacterCommand(EntityId Id, GamePosition Target) : ICommand
    {
    }
}
