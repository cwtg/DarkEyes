using System;
using System.Collections.Generic;
using System.Linq;
using DarkEyes.Backend.Commands;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.Space.Pathfinding;

namespace DarkEyes.Backend.Rx
{
    public sealed class MoveCharacterHandler : ICommandHandler<MoveCharacterCommand>
    {
        public void Handle(MoveCharacterCommand command, HandlerContext context)
        {
            try
            {
                Entity entity = context.Store.World.Entities.Get(command.Id);

                if (entity.Position == command.Target)
                {
                    return;
                }

                IReadOnlyList<GamePosition>? path = command.PreferredPath;
                INavigationView navigationView = context.Store.World.NavigationSnapshot();
                if (!ValidateHint(entity.Position, path, command.Target, navigationView, entity.MovementProfile))
                {
                    path = Pathfinder.FindPath(entity.Position, command.Target, navigationView, entity.MovementProfile);
                }

                if (path.Count == 0)
                {
                    context.Emit(new ErrorRaisedEvent($"No valid path to target {command.Target} for {command.Id}"));
                }

                GamePosition from = entity.Position;
                foreach (GamePosition cell in path)
                {
                    CellContext cellContext = navigationView.ContextAt(cell);
                    bool interrupted = false; // evaluate cellContext
                    if (interrupted)
                    {
                        return;
                    }

                    context.Store.World.ChangePosition(command.Id, cell);

                    context.Emit(new CharacterSteppedEvent(command.Id, from, cell, cellContext));
                    from = cell;
                }
            }
            catch (KeyNotFoundException)
            {
                context.Emit(new ErrorRaisedEvent($"Entity {command.Id} not registered"));
            }
        }

        private static bool ValidateHint(GamePosition from, IReadOnlyList<GamePosition> path, GamePosition target, INavigationView navigationView, MovementProfile profile)
        {
            if (path is null || path.Count == 0) return false;

            GamePosition previous = from;
            int movementCost = 0;
            foreach (GamePosition current in path)
            {
                if (!previous.Neighbors().Contains(current)) return false;
                if (!IsPositionValid(current, navigationView)) return false;

                movementCost += navigationView.MovementCost(current);
                previous = current;
            }

            if (previous != target) return false;
            if (movementCost > profile.MaxRange) return false;

            return true;
        }

        private static bool IsPositionValid(GamePosition position, INavigationView navigationView)
        {
            return navigationView.IsWalkable(position) && !navigationView.IsOccupied(position);
        }
    }
}
