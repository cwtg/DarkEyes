using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Tests.Support;

internal sealed class DecreeHandler : IActionHandler<DecreeAction>
{
    public Rejection? Handle(DecreeAction action, ActionContext context)
    {
        context.Submit(action.Effect);
        return null;
    }
}
