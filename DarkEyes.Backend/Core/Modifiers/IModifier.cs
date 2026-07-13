using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifiers
{
    public interface IModifier
    {
        public int Apply(int input);
        public StatId Target { get; }
        public int Order { get; }
        public bool IsActive(in ModifierContext context);
    }
}
