using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Locks;

public sealed record LockUnlocked(EntityId Target, EntityId UnlockedBy) : IEvent;
