namespace DarkEyes.Backend.Pipeline;

public interface IActionHandler<in TAction>
    where TAction : IAction
{
    void Handle(TAction action, ActionContext context);
}
