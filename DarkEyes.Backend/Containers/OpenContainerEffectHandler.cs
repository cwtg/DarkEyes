using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Containers;

internal sealed class OpenContainerEffectHandler : IEffectHandler<OpenContainerEffect>
{
    public void Handle(OpenContainerEffect effect, EffectContext context)
    {
        ContainerState container = context.State.GetEntity(effect.Container).Get(ContainerState.Key);
        context.SetComponent(effect.Container, ContainerState.Key, container with { IsOpen = true });
        context.Emit(new ContainerOpened(effect.Container, effect.OpenedBy));
    }
}
