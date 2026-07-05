namespace DarkEyes.Backend.Abstractions
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
        public void Log(LogLevel lvel, string message);
    }
}
