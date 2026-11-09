using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Containers;

public sealed record OpenContainerAction(EntityId Actor, EntityId Container) : IAction;
