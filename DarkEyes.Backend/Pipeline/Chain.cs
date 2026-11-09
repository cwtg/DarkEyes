using System;

namespace DarkEyes.Backend.Pipeline;

/// <summary>One run of the pipeline: a root and everything that follows from it, against one transaction.</summary>
internal sealed class Chain(Transaction transaction, ActionRouter actions, EffectRouter effects)
{
    private readonly Transaction _transaction = transaction;
    private readonly ActionRouter _actions = actions;
    private readonly EffectRouter _effects = effects;
    private bool _changed;

    public Transaction Transaction => _transaction;

    /// <summary>Runs the root action. A rejection is pure validation: it is only valid before the first submitted effect or emitted event.</summary>
    public Rejection? Run(IAction root)
    {
        Rejection? rejection = _actions.Handle(root, new ActionContext(this));
        if (rejection is not null && _changed)
        {
            throw new InvalidOperationException($"{root.GetType().Name} was rejected ({rejection}) after it had already submitted effects or emitted events. Validate before changing anything.");
        }

        return rejection;
    }

    public void Submit(IEffect effect)
    {
        _changed = true;
        _effects.Apply(effect, new EffectContext(_transaction));
    }

    public void Emit(IEvent gameEvent)
    {
        _changed = true;
        _transaction.Emit(gameEvent);
    }
}
