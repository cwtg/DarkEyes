using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.LockPicking;

internal sealed class PickLockHandler : IActionHandler<PickLockAction>
{
    /// <summary>Picking a lock needs the actor on a neighbouring hex.</summary>
    private const int MaxDistance = 1;

    public Reason? Handle(PickLockAction action, ActionContext context)
    {
        Entity target = context.State.GetEntity(action.Target);
        if (!target.Has(Lock.Key))
        {
            throw new ArgumentException($"{action.Target} has no lock; a compliant caller never sends this.", nameof(action));
        }

        int distance = PlacementDistance.Between(context.State, action.Actor, action.Target);
        if (distance > MaxDistance)
        {
            return PickLockRejections.TooFarAway(distance, MaxDistance);
        }

        // After the range check: from afar, a rejection must not reveal whether something is locked.
        if (!target.Get(Lock.Key).IsLocked)
        {
            return PickLockRejections.NotLocked();
        }

        context.Check(new CheckRequest(CheckPurposes.PickLock, action.Actor, action.Target)).Match(
            succeeded: _ => context.Submit(new UnlockEffect(action.Target, action.Actor)),
            failed: () => { },
            notPossible: reason => context.Emit(new LockPickNotPossible(action.Actor, action.Target, reason)));

        return null;
    }
}
