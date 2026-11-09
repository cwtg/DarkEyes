using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.LockPicking;

public sealed record PickLockAction(EntityId Actor, EntityId Target) : IAction;
