using DarkEyes.Backend.Commands;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Rx;

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
            return new GameOrchestrator(router, new(), new(), logger);
        }

        private void RegisterHandlers()
        {
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
        }
    }
}
