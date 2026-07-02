using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend.Rx
{
    public sealed class ErrorRaisedEvent(string reason) : IGameEvent
    {
        public readonly string Reason = reason;
    }
}
