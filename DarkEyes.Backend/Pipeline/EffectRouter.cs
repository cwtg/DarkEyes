using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Pipeline;

/// <summary>Finds the handler for an effect. Types are lookup keys only (no reflection).</summary>
internal sealed class EffectRouter
{
    private readonly Dictionary<Type, Action<IEffect, EffectContext>> _handlers = [];

    public void Add<TEffect>(IEffectHandler<TEffect> handler)
        where TEffect : IEffect
    {
        _handlers.Add(typeof(TEffect), (effect, context) => handler.Handle((TEffect)effect, context));
    }

    public void Apply(IEffect effect, EffectContext context)
    {
        if (!_handlers.TryGetValue(effect.GetType(), out Action<IEffect, EffectContext>? handler))
        {
            throw new InvalidOperationException($"No handler registered for effect {effect.GetType().Name}.");
        }

        handler(effect, context);
    }
}
