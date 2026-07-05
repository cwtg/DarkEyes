using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Rx;
using DarkEyes.Backend.Space.Commands;

namespace DarkEyes.Backend
{
    public sealed class GameBackendBuilder
    {
        private readonly CommandRouter router = new();
        private ILogger logger = new NoopLogger();

        public GameBackendBuilder WithLogger(ILogger logger)
        {
            this.logger = logger;
            return this;
        }

        public IGameOrchestrator Build()
        {
            RegisterHandlers();
            return new GameOrchestrator(router, new(), logger);
        }

        private void RegisterHandlers()
        {
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
        }
    }
}
