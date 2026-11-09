namespace DarkEyes.Backend.Logging;

public interface IGameLogger
{
    void Log(LogLevel level, string message);
}
