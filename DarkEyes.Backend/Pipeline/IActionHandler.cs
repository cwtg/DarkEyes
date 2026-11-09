namespace DarkEyes.Backend.Pipeline;

public interface IActionHandler<in TAction>
    where TAction : IAction
{
    /// <summary>Handles the action and returns its rejection or its outcome; the pipeline reports the outcome.</summary>
    ActResult Handle(TAction action, ActionContext context);
}
