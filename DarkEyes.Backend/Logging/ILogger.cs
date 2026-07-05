namespace DarkEyes.Backend.Logging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public interface ILogger
    {
        public void Log(LogLevel level, string message);
    }
}
