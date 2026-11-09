namespace DarkEyes.Backend.Pipeline;

public interface IEffectHandler<in TEffect>
    where TEffect : IEffect
{
    void Handle(TEffect effect, EffectContext context);
}
