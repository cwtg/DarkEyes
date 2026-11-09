using DarkEyes.Backend.Randomness;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>A record-level write to the state.</summary>
internal interface IOp
{
}

internal sealed record SetComponentOp(EntityId Entity, ComponentKey Key, object Component) : IOp;

internal sealed record SetRandomStreamOp(RandomStreamId Stream, Pcg32State State) : IOp;
