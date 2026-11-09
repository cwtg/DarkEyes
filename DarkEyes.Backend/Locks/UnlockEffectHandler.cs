using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Locks;

/// <summary>Idempotent: an unlocked lock stays as it is, without an event (decreeing callers such as the story need no check first).</summary>
internal sealed class UnlockEffectHandler : IEffectHandler<UnlockEffect>
{
    public void Handle(UnlockEffect effect, EffectContext context)
    {
        Lock targetLock = context.State.GetEntity(effect.Target).Get(Lock.Key);
        if (!targetLock.IsLocked)
        {
            return;
        }

        context.SetComponent(effect.Target, Lock.Key, targetLock with { IsLocked = false });
        context.Emit(new LockUnlocked(effect.Target, effect.UnlockedBy));
    }
}
