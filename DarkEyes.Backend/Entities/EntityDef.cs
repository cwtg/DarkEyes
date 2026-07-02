using System.Collections.Generic;

namespace DarkEyes.Backend.Entities
{
    public record EntityDef(EntityId Id, string DisplayNameId, IReadOnlyList<ComponentData> Components)
    {
    }
}
