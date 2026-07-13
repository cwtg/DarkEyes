namespace DarkEyes.Backend.Logging
{
    public sealed class NullLogger : IGameLogger
    {
        public void Log(LogLevel level, string message)
        {
        }
    }
}
