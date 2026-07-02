namespace DarkEyes.Backend.Abstractions
{
    public interface ILogger
    {
        public void Log(LogLevel lvel, string message);
    }

    public enum LogLevel { Debug, Info, Warning, Error }
}
