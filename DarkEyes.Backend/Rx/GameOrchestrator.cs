using DarkEyes.Backend.Abstractions;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DarkEyes.Backend.Rx
{
    public sealed class GameOrchestrator(CommandRouter router, GameStore store, ILogger logger) : IGameOrchestrator
    {
        private readonly CommandRouter _router = router;
        private readonly GameStore _store = store;
        private readonly ILogger _logger = logger;

        private readonly Subject<IGameEvent> _subject = new();
        private bool _dispatching = false;

        public IObservable<IGameEvent> Events => _subject.AsObservable();

        public void Dispatch(ICommand command)
        {
            if (_dispatching) throw new InvalidOperationException("Re-entrant dispatch is not allowed.");

            _dispatching = true;
            try
            {
                HandlerContext context = new(emit: _subject.OnNext, _store, _logger);
                _router.Handle(command, context);
            }
            finally
            { 
                _dispatching = false;
            }
        }
    }
}
