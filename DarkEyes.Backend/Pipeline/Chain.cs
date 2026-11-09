namespace DarkEyes.Backend.Pipeline;

/// <summary>One run of the pipeline: a root and everything that follows from it, against one transaction.</summary>
internal sealed class Chain(Transaction transaction, ActionRouter actions, EffectRouter effects)
{
    private readonly Transaction _transaction = transaction;
    private readonly ActionRouter _actions = actions;
    private readonly EffectRouter _effects = effects;

    public CommitResult Run(IAction root)
    {
        _actions.Handle(root, new ActionContext(_transaction, this));
        return _transaction.Complete();
    }

    public void Submit(IEffect effect)
    {
        _effects.Apply(effect, new EffectContext(_transaction));
    }
}
