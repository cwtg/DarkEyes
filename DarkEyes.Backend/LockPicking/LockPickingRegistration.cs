using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.LockPicking;

public static class LockPickingRegistration
{
    public static GameStoreBuilder AddLockPicking(this GameStoreBuilder builder)
    {
        return builder.AddActionHandler(new PickLockHandler());
    }
}
