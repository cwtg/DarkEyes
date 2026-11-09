using System;
using DarkEyes.Backend.Checks;

namespace DarkEyes.Backend.Pipeline;

/// <summary>One run of the pipeline: a root and everything that follows from it, against one transaction.</summary>
internal sealed class Chain(Transaction transaction, PipelineServices services)
{
    private readonly Transaction _transaction = transaction;
    private readonly PipelineServices _services = services;
    private bool _changed;

    public Transaction Transaction => _transaction;

    /// <summary>
    /// Runs the root action and returns its rejection, or null if it was carried out; then its outcome is the chain's last event.
    /// A rejection is pure validation: it is only valid before the first check, submitted effect or emitted event.
    /// </summary>
    public Reason? Run(IAction root)
    {
        ActResult result = _services.Actions.Handle(root, new ActionContext(this))
            ?? throw new InvalidOperationException($"The handler for {root.GetType().Name} returned no result.");
        if (result.Rejection is { } rejection)
        {
            if (_changed)
            {
                throw new InvalidOperationException($"{root.GetType().Name} was rejected ({rejection}) after it had already run a check, submitted effects or emitted events. Validate before changing anything.");
            }

            return rejection;
        }

        _transaction.RequireNoUnexplainedChanges();
        Emit(new ActionCompleted(root, result.Outcome!));
        return null;
    }

    public CheckResult Check(CheckRequest request)
    {
        _changed = true;
        return RequireChecks(request).Roll(request, new CheckContext(this));
    }

    private IChecks RequireChecks(CheckRequest request) => _services.Checks
        ?? throw new InvalidOperationException($"A check ({request.Purpose}) was requested, but no check service is installed. The rule module installs it.");

    public void Submit(IEffect effect)
    {
        _changed = true;
        _services.Effects.Apply(effect, new EffectContext(this));
    }

    public void Emit(IEvent gameEvent)
    {
        _changed = true;
        _transaction.Emit(gameEvent);
    }
}
