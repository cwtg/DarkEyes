using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution
{
    public readonly ref struct PreRollContext(StatId subject, int effectivePool)
    {
        public readonly StatId Subject = subject;
        public readonly int EffectivePool = effectivePool;
    }
}
