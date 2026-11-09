using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifiers;

public interface IModifier
{
    int Apply(int input);
    StatId Target { get; }
    int Order { get; }
    bool IsActive(in ModifierContext context);
}
