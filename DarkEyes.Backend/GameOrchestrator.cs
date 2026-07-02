using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Rx;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace DarkEyes.Backend
{
    public sealed class GameOrchestrator(CommandRouter router, ILogger logger) : IGameOrchestrator
    {
        private readonly CommandRouter _router = router;
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
                HandlerContext context = new(emit: e => _subject.OnNext(e), _logger);
                _router.Handle(command, context);
            }
            finally
            { 
                _dispatching = false;
            }
        }
    }
}
