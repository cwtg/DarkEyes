#pragma warning disable CA1051 // Legacy code from before the architecture; replaced when its use case is ported (architecture §11.2).

using System;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Store;

namespace DarkEyes.Backend.Orchestration;

public sealed class HandlerContext(Action<IGameEvent> emit, GameStore store, IRandomSource random, IGameLogger logger)
{
    public readonly IGameLogger Logger = logger;
    public readonly GameStore Store = store;
    public readonly Action<IGameEvent> Emit = emit;
    public readonly IRandomSource Random = random;
}
