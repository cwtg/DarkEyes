using DarkEyes.Backend.Orchestration;

namespace DarkEyes.Backend.Handlers;

public delegate void CommandHandler<in T>(T command, HandlerContext context) where T : ICommand;

public interface ICommandHandler<in T> where T : ICommand
{
    void Handle(T command, HandlerContext context);
}
