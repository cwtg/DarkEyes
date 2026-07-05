namespace DarkEyes.Backend.Core.Stats
{
    public sealed class Attribute(StatId id, int value) : IStat
    {
        public StatId Id { get; } = id;
        public int Value { get; } = value;
    }
}
