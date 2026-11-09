using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>Lets a test decree an effect directly, as story, scheduler or reactions will (they enter at effects, unvalidated).</summary>
internal sealed record DecreeAction(IEffect Effect) : IAction
{
    public EntityId Actor { get; } = new("test.decree");
}
