using System;
using System.Collections.Generic;
using DarkEyes.Backend.Commands;
using DarkEyes.Backend.Events;

namespace DarkEyes.Backend.Rx
{
    public sealed class CommandRouter
    {
        private readonly Dictionary<Type, CommandHandler<ICommand>> _handlers = [];

        public void Register<T>(CommandHandler<T> handler)
            where T : ICommand
        {
            _handlers[typeof(T)] = (command, context) => handler((T)command, context);
        }

        public void Handle(ICommand command, HandlerContext context)
        {
            if (_handlers.TryGetValue(command.GetType(), out CommandHandler<ICommand>? handler))
                handler(command, context);
            else
                context.Emit(new ErrorRaisedEvent($"No handler for {command.GetType().Name}"));
        }
    }
}
