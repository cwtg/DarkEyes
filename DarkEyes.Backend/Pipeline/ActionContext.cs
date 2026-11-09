using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>What an action handler may do: read the transaction view, submit effects, emit events. No direct writes.</summary>
public sealed class ActionContext
{
    private readonly Chain _chain;

    internal ActionContext(Chain chain)
    {
        _chain = chain;
    }

    public GameState State => _chain.Transaction.State;

    public void Submit(IEffect effect) => _chain.Submit(effect);

    public void Emit(IEvent gameEvent) => _chain.Emit(gameEvent);
}
