namespace DarkEyes.Backend.Events
{
    public sealed class ErrorRaisedEvent(string reason) : IGameEvent
    {
        public readonly string Reason = reason;
    }
}
