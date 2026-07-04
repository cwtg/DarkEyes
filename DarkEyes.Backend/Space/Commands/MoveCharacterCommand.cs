using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Entities;
using System.Collections.Generic;

namespace DarkEyes.Backend.Space.Commands
{
    public record MoveCharacterCommand(EntityId Id, GamePosition Target, IReadOnlyList<GamePosition>? PreferredPath) : ICommand
    {
    }
}
