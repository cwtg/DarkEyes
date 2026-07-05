using System.Collections.Generic;
using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Entities;

namespace DarkEyes.Backend.Space.Commands
{
    public record MoveCharacterCommand(EntityId Id, GamePosition Target, IReadOnlyList<GamePosition>? PreferredPath) : ICommand
    {
    }
}
