using System;

namespace DarkEyes.Backend.Core.Resolution
{
    public sealed class SeededRandomSource(int seed) : IRandomSource
    {
        private readonly Random _random = new(seed);

        public int Next()
        {
            return _random.Next();
        }

        public int Next(int max)
        {
            return _random.Next(max);
        }

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
