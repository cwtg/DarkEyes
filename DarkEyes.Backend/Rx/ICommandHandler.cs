using DarkEyes.Backend.Commands;

namespace DarkEyes.Backend.Rx
{
    public delegate void CommandHandler<in T>(T command, HandlerContext context) where T : ICommand;

    public interface ICommandHandler<in T> where T : ICommand
    {
        public void Handle(T command, HandlerContext context);
    }
}
