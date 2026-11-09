using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.LockPicking;

/// <summary>The rules don't allow this actor to pick this lock (e.g. too hard for them); the reason is the rule module's.</summary>
public sealed record LockPickNotPossible(EntityId Actor, EntityId Target, Reason Reason) : IEvent;
