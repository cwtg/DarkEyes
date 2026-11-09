using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Containers;

public static class ContainersRegistration
{
    public static GameStoreBuilder AddContainers(this GameStoreBuilder builder)
    {
        return builder
            .AddActionHandler(new OpenContainerHandler())
            .AddEffectHandler(new OpenContainerEffectHandler());
    }
}
