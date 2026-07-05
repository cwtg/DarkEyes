namespace DarkEyes.Backend.Core.Resolution
{
    public interface IRandomSource
    {
        public int Next();

        public int Next(int max);

        public int Next(int min, int max);
    }
}
