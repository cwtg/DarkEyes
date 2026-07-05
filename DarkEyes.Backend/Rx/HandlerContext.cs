using System;
using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend.Rx
{
    public sealed class HandlerContext(Action<IGameEvent> emit, GameStore store, ILogger logger)
    {
        public readonly ILogger Logger = logger;
        public readonly GameStore Store = store;
        public readonly Action<IGameEvent> Emit = emit;
    }
}
