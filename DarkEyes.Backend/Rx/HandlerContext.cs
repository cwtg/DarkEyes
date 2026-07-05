using System;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Rx
{
    public sealed class HandlerContext(Action<IGameEvent> emit, GameStore store, LogicComponentRegistry registry, ILogger logger)
    {
        public readonly ILogger Logger = logger;
        public readonly GameStore Store = store;
        public readonly Action<IGameEvent> Emit = emit;
        public readonly LogicComponentRegistry Registry = registry;
    }
}
