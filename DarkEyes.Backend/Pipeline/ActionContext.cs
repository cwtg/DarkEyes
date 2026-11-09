using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>What an action handler may do: read the transaction view, request checks, submit effects, emit events. No direct writes.</summary>
public sealed class ActionContext
{
    private readonly Chain _chain;

    internal ActionContext(Chain chain)
    {
        _chain = chain;
    }

    public GameState State => _chain.Transaction.State;

    /// <summary>Runs a check through the rule module. Like an effect or event, it counts as a change: no rejection afterwards.</summary>
    public CheckResult Check(CheckRequest request) => _chain.Check(request);

    public void Submit(IEffect effect) => _chain.Submit(effect);

    public void Emit(IEvent gameEvent) => _chain.Emit(gameEvent);
}
