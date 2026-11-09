using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Pipeline;

/// <summary>Finds the handler for an action. Types are lookup keys only (no reflection).</summary>
internal sealed class ActionRouter
{
    private readonly Dictionary<Type, Action<IAction, ActionContext>> _handlers = [];

    public void Add<TAction>(IActionHandler<TAction> handler)
        where TAction : IAction
    {
        _handlers.Add(typeof(TAction), (action, context) => handler.Handle((TAction)action, context));
    }

    public void Handle(IAction action, ActionContext context)
    {
        if (!_handlers.TryGetValue(action.GetType(), out Action<IAction, ActionContext>? handler))
        {
            throw new InvalidOperationException($"No handler registered for action {action.GetType().Name}.");
        }

        handler(action, context);
    }
}
