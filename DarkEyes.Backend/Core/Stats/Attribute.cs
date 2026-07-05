namespace DarkEyes.Backend.Core.Stats
{
    public sealed class Attribute(StatId id) : IStat
    {
        public StatId Id { get; } = id;
        public int Value { get; set; }

        public void SetValue(int val)
        {
            Value = val;
        }
    }
}
