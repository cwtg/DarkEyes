using DarkEyes.Backend.Rx;
using DarkEyes.Backend.Space.Events;

namespace DarkEyes.Backend.Space.Commands
{
    public sealed class MoveCharacterHandler : ICommandHandler<MoveCharacterCommand>
    {
        public void Handle(MoveCharacterCommand command, HandlerContext context)
        {

            var entityExists = context.Store.World.EntityPositions.TryGetValue(command.Id, out var from);
            if (entityExists)
            {
                context.Store.World.ChangePosition(command.Id, command.Target);
                context.Emit(new CharacterSteppedEvent(command.Id, from, command.Target));
            }
            else
                context.Emit(new ErrorRaisedEvent($"Entity {command.Id} not registered"));
        }
    }
}
