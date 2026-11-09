using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>An attempt by an actor (player or AI); rule-checked by its handler.</summary>
public interface IAction
{
    EntityId Actor { get; }
}
