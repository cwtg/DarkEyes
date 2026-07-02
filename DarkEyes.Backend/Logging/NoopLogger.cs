using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend.Logging
{
    public class NoopLogger : ILogger
    {
        public void Log(LogLevel lvel, string message) { }
    }
}
