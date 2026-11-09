using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Tests.Support;

internal sealed class DecreeHandler : IActionHandler<DecreeAction>
{
    public Reason? Handle(DecreeAction action, ActionContext context)
    {
        context.Submit(action.Effect);
        return null;
    }
}
