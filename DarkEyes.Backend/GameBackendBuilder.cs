using DarkEyes.Backend.Handlers;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Orchestration;

namespace DarkEyes.Backend;

public sealed class GameBackendBuilder
{
    private readonly CommandRouter _router = new();
    private IGameLogger _logger = new NullLogger();

    public GameBackendBuilder WithLogger(IGameLogger logger)
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
