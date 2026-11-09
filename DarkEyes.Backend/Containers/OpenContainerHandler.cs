using System;
using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Containers;

internal sealed class OpenContainerHandler : IActionHandler<OpenContainerAction>
{
    /// <summary>Interim reach until footprints exist: the actor must stand on a neighbouring hex.</summary>
    private const int MaxDistance = 1;

    public Rejection? Handle(OpenContainerAction action, ActionContext context)
    {
        Entity container = context.State.GetEntity(action.Container);
        if (!container.Has(ContainerState.Key))
        {
            throw new ArgumentException($"{action.Container} is not a container; a compliant caller never sends this.", nameof(action));
        }

        if (container.Get(ContainerState.Key).IsOpen)
        {
            return ContainerRejections.AlreadyOpen();
        }

        GamePosition actorPosition = context.State.GetEntity(action.Actor).Get(Placement.Key).Position;
        GamePosition containerPosition = container.Get(Placement.Key).Position;

        int distance = actorPosition.DistanceTo(containerPosition);
        if (distance > MaxDistance)
        {
            return ContainerRejections.TooFarAway(distance, MaxDistance);
        }

        // After the range check: from afar, a rejection must not reveal whether something is locked.
        if (container.Has(Lock.Key) && container.Get(Lock.Key).IsLocked)
        {
            return ContainerRejections.Locked();
        }

        context.Submit(new OpenContainerEffect(action.Container, action.Actor));
        return null;
    }
}
