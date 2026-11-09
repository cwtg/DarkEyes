using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Containers;

public sealed record ContainerOpened(EntityId Container, EntityId OpenedBy) : IEvent;
