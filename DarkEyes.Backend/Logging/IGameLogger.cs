namespace DarkEyes.Backend.Logging
{
    public interface IGameLogger
    {
        public void Log(LogLevel level, string message);
    }
}
