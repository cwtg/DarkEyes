using DarkEyes.Backend.Rx;
using DarkEyes.Backend.Space.Events;
using System.Collections.Generic;

namespace DarkEyes.Backend.Space.Commands
{
    public sealed class MoveCharacterHandler : ICommandHandler<MoveCharacterCommand>
    {
        public void Handle(MoveCharacterCommand command, HandlerContext context)
        {
            try
            {
                var entity = context.Store.World.Entities.Get(command.Id);
                context.Store.World.ChangePosition(command.Id, command.Target);
                context.Emit(new CharacterSteppedEvent(command.Id, entity.Position, command.Target));
            }
            catch (KeyNotFoundException ex)
            {
                context.Emit(new ErrorRaisedEvent($"Entity {command.Id} not registered"));
            }
        }
    }
}
