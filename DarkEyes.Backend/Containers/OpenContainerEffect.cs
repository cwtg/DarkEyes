using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Containers;

public sealed record OpenContainerEffect(EntityId Container, EntityId OpenedBy) : IEffect;
