using DarkEyes.Backend.Abstractions;
using System;

namespace DarkEyes.Backend.Rx
{
    public sealed class HandlerContext(Action<IGameEvent> emit, ILogger logger)
    {
        public readonly ILogger Logger = logger;
        public readonly Action<IGameEvent> Emit = emit;
    }
}
