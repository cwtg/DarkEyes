using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Rx;
using DarkEyes.Backend.Space.Commands;

namespace DarkEyes.Backend
{
    public sealed class GameBackendBuilder
    {
        private readonly CommandRouter _router = new();
        private ILogger _logger = new NoopLogger();

        public GameBackendBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public IGameOrchestrator Build()
        {
            RegisterHandlers();
            return new GameOrchestrator(_router, new(), _logger);
        }

        private void RegisterHandlers()
        {
            _router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
        }
    }
}
