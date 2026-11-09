using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Containers;

internal sealed class OpenContainerHandler : IActionHandler<OpenContainerAction>
{
    public void Handle(OpenContainerAction action, ActionContext context)
    {
        context.Submit(new OpenContainerEffect(action.Container, action.Actor));
    }
}
