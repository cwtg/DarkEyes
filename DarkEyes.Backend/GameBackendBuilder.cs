using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Rx;
using DarkEyes.Backend.Time;
using System;
using System.Collections.Generic;

namespace DarkEyes.Backend
{
    public sealed class GameBackendBuilder
    {
        private readonly CommandRouter _router = new();
        private readonly Dictionary<Type, (ITickEvaluator, TickPhase)> _evaluators = [];
        private ILogger _logger = new NoopLogger();

        public GameBackendBuilder RegisterHandler<T>(Action<T, HandlerContext> handler) where T : ICommand
        {
            _router.Register<T>(handler);
            return this;
        }

        public GameBackendBuilder RegisterTickEvaluator<T>(T evaluator, TickPhase phase) where T : ITickEvaluator
        {
            _evaluators[typeof(T)] = (evaluator, phase);
            return this;
        }

        public GameBackendBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public IGameOrchestrator Build()
        {
            return new GameOrchestrator(_router, new(), _logger);
        }
    }
}
