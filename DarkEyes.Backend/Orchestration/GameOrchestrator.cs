#pragma warning disable CA1001 // Legacy code, replaced or moved to the rule module in story 01 (docs/implementation/01-open-chest.md).

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Handlers;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Store;

namespace DarkEyes.Backend.Orchestration;

public sealed class GameOrchestrator(CommandRouter router, GameStore store, IRandomSource random, IGameLogger logger) : IGameOrchestrator
{
    private readonly CommandRouter _router = router;
    private readonly GameStore _store = store;
    private readonly IRandomSource _random = random;
    private readonly IGameLogger _logger = logger;

    private readonly Subject<IGameEvent> _subject = new();
    private bool _dispatching;

    public IObservable<IGameEvent> Events => _subject.AsObservable();

    public void Dispatch(ICommand command)
    {
        if (_dispatching)
        {
            throw new InvalidOperationException("Re-entrant dispatch is not allowed.");
        }

        _dispatching = true;
        try
        {
            HandlerContext context = new(emit: _subject.OnNext, _store, _random, _logger);
            _router.Handle(command, context);
        }
        finally
        {
            _dispatching = false;
        }
    }
}
