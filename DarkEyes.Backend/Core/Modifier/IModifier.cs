using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifier
{
    public enum CombinationType : byte
    {
        Additive,
        Multiplicative,
    }

    public interface IModifier
    {
        public CombinationType Combination { get; }
        public int Value { get; }
        public StatId AffectedStat { get; }
    }

    public interface IModifier<in T> : IModifier
    {
        public IActivationCondition<T> Condition { get; }
    }
}
