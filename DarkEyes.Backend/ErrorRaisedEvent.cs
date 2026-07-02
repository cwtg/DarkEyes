using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend
{
    public sealed class ErrorRaisedEvent(string reason) : IGameEvent
    {
        public readonly string Reason = reason;
    }
}
