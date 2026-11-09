using System.Collections.Generic;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Handlers;

public record MoveCharacterCommand(EntityId Id, GamePosition Target, IReadOnlyList<GamePosition>? PreferredPath) : ICommand
{
}
