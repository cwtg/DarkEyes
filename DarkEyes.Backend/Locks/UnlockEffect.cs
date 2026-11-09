using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Locks;

public sealed record UnlockEffect(EntityId Target, EntityId UnlockedBy) : IEffect;
