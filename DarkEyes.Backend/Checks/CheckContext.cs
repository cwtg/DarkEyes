using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Checks;

/// <summary>What a check may do: read the state and report its own event and consequences. It runs in the chain that requests it, from an action or an effect.</summary>
public sealed class CheckContext
{
    private readonly Chain _chain;

    internal CheckContext(Chain chain)
    {
        _chain = chain;
    }

    public GameState State => _chain.Transaction.State;

    public void Submit(IEffect effect) => _chain.Submit(effect);

    public void Emit(IEvent gameEvent) => _chain.Emit(gameEvent);
}
