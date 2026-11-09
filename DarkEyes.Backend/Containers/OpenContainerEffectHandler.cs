using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Containers;

/// <summary>Idempotent: an open container stays as it is, without an event (decreeing callers such as the story need no check first).</summary>
internal sealed class OpenContainerEffectHandler : IEffectHandler<OpenContainerEffect>
{
    public void Handle(OpenContainerEffect effect, EffectContext context)
    {
        ContainerState container = context.State.GetEntity(effect.Container).Get(ContainerState.Key);
        if (container.IsOpen)
        {
            return;
        }

        context.SetComponent(effect.Container, ContainerState.Key, container with { IsOpen = true });
        context.Emit(new ContainerOpened(effect.Container, effect.OpenedBy));
    }
}
