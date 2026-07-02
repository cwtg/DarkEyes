using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend.Rx
{
    public delegate void CommandHandler<T>(T command, HandlerContext context) where T : ICommand;

    public interface ICommandHandler<in T> where T : ICommand
    {
        public void Handle(T command, HandlerContext context);
    }
}
