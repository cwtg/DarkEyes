namespace DarkEyes.Backend.Pipeline;

public interface IActionHandler<in TAction>
    where TAction : IAction
{
    /// <summary>Handles the action; returns a rejection to refuse it (the whole chain is then discarded), or null.</summary>
    Rejection? Handle(TAction action, ActionContext context);
}
