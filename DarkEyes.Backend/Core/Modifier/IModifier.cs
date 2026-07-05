using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifier
{
    public interface IModifier
    {
        public int Apply(int input);
        public StatId Target { get; }
        public int Order { get; }
    }
}
