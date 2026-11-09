using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>What an effect handler may do: read the transaction view, write ops, emit events.</summary>
public sealed class EffectContext
{
    private readonly Transaction _transaction;

    internal EffectContext(Transaction transaction)
    {
        _transaction = transaction;
    }

    public GameState State => _transaction.State;

    public void SetComponent<T>(EntityId entity, ComponentKey<T> key, T component)
        where T : class
    {
        _transaction.Apply(new SetComponentOp(entity, key, component));
    }

    public void Emit(IEvent gameEvent) => _transaction.Emit(gameEvent);
}
