using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Entities;

namespace DarkEyes.Backend.Events;

public sealed class ResolutionRolledEvent(EntityId entityId, ResultType type, int qualitiyLevel) : IGameEvent
{
    public EntityId EntityId { get; } = entityId;
    public ResultType Type { get; } = type;
    public int QualitiyLevel { get; } = qualitiyLevel;
}
