using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>What an effect handler may do: read the transaction view, request checks, write ops, emit events.</summary>
public sealed class EffectContext
{
    private readonly Chain _chain;

    internal EffectContext(Chain chain)
    {
        _chain = chain;
    }

    public GameState State => _chain.Transaction.State;

    /// <summary>Runs a check through the rule module, e.g. a trap or a resistance against poison.</summary>
    public CheckResult Check(CheckRequest request) => _chain.Check(request);

    public void SetComponent<T>(EntityId entity, ComponentKey<T> key, T component)
        where T : class
    {
        _chain.Transaction.Apply(new SetComponentOp(entity, key, component));
    }

    public void Emit(IEvent gameEvent) => _chain.Emit(gameEvent);
}
