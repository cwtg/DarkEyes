namespace DarkEyes.Backend.Logging
{
    public class NoopLogger : ILogger
    {
        public void Log(LogLevel level, string message)
        {
        }
    }
}
