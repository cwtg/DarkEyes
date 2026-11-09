using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Handlers;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Orchestration;

namespace DarkEyes.Backend;

public sealed class GameBackendBuilder
{
    private readonly CommandRouter _router = new();
    private IGameLogger _logger = new NullLogger();
    private IRandomSource _random = new SeededRandomSource(null);

    public GameBackendBuilder WithLogger(IGameLogger logger)
    {
        _logger = logger;
        return this;
    }

    public GameBackendBuilder WithRandomSource(IRandomSource random)
    {
        _random = random;
        return this;
    }

    public IGameOrchestrator Build()
    {
        RegisterHandlers();
        return new GameOrchestrator(_router, new(), _random, _logger);
    }

    private void RegisterHandlers()
    {
        _router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
        _router.Register<OpenContainerCommand>(new OpenContainerHandler().Handle);
    }
}
