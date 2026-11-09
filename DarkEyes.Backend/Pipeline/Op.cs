using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>A record-level write to the state.</summary>
internal abstract record Op(EntityId Entity);

internal sealed record SetComponentOp(EntityId Entity, ComponentKey Key, object Component) : Op(Entity);
