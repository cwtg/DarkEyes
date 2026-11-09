using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>What an action handler may do: read the transaction view, submit effects, emit events. No direct writes.</summary>
public sealed class ActionContext
{
    private readonly Transaction _transaction;
    private readonly Chain _chain;

    internal ActionContext(Transaction transaction, Chain chain)
    {
        _transaction = transaction;
        _chain = chain;
    }

    public GameState State => _transaction.State;

    public void Submit(IEffect effect) => _chain.Submit(effect);

    public void Emit(IEvent gameEvent) => _transaction.Emit(gameEvent);
}
