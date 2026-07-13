using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifiers
{
    public readonly ref struct ModifierContext(StatId subject)
    {
        public readonly StatId Subject = subject;
    }
}
