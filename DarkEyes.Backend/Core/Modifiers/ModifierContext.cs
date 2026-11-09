#pragma warning disable CA1051 // Legacy code, replaced or moved to the rule module in story 01 (docs/implementation/01-open-chest.md).

using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifiers;

public readonly ref struct ModifierContext(StatId subject)
{
    public readonly StatId Subject = subject;
}
