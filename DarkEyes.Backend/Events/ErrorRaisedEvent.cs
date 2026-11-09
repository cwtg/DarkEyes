namespace DarkEyes.Backend.Events;

public sealed class ErrorRaisedEvent(string reason) : IGameEvent
{
    public string Reason { get; } = reason;
}
