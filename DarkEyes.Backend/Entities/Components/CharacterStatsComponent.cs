using System.Collections.Generic;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Entities.Components
{
    public class CharacterStatsComponent(string componentId) : ILogicComponent
    {
        public string ComponentId { get; } = componentId;
        public IReadOnlyList<Attribute> Attributes { get; } = [];
        public IReadOnlyList<Skill> Skills { get; } = [];
    }
}
