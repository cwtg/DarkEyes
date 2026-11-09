using System;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

public sealed class GameStoreBuilder
{
    private readonly ActionRouter _actions = new();
    private readonly EffectRouter _effects = new();
    private bool _built;

    public GameStoreBuilder AddActionHandler<TAction>(IActionHandler<TAction> handler)
        where TAction : IAction
    {
        ThrowIfBuilt();
        _actions.Add(handler);
        return this;
    }

    public GameStoreBuilder AddEffectHandler<TEffect>(IEffectHandler<TEffect> handler)
        where TEffect : IEffect
    {
        ThrowIfBuilt();
        _effects.Add(handler);
        return this;
    }

    /// <summary>Builds the store. A builder builds exactly one store; the routers are handed over, not shared.</summary>
    public GameStore Build(GameState state)
    {
        ThrowIfBuilt();
        _built = true;
        return new GameStore(state, _actions, _effects);
    }

    private void ThrowIfBuilt()
    {
        if (_built)
        {
            throw new InvalidOperationException("This builder has already built a store.");
        }
    }
}
