namespace DarkEyes.Backend.Core.Stats
{
    public interface IStat
    {
        public StatId Id { get; }

        public int Value { get; }
    }
}
