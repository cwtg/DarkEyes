using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Locks;

public static class LocksRegistration
{
    public static GameStoreBuilder AddLocks(this GameStoreBuilder builder)
    {
        return builder.AddEffectHandler(new UnlockEffectHandler());
    }
}
