using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend.Tests
{
    internal class NoopLogger : ILogger
    {
        public void Log(LogLevel lvel, string message) { }
    }
}
