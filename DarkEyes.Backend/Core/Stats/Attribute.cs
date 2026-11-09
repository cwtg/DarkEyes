#pragma warning disable CA1711 // Legacy code, replaced or moved to the rule module in story 01 (docs/implementation/01-open-chest.md).

namespace DarkEyes.Backend.Core.Stats;

public sealed class Attribute(StatId id, int value) : IStat
{
    public StatId Id { get; } = id;
    public int Value { get; } = value;
}
