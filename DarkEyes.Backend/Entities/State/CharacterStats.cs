using System.Collections.Generic;
using DarkEyes.Backend.Core.Stats;
using System.Linq;

namespace DarkEyes.Backend.Entities.State
{
    public class CharacterStats(string componentId, IReadOnlyList<Attribute> attributes, IReadOnlyList<Skill> skills)
    {
        public string ComponentId { get; } = componentId;
        public IReadOnlyDictionary<StatId, Attribute> Attributes { get; } = attributes.ToDictionary(attribute => attribute.Id);
        public IReadOnlyDictionary<StatId, Skill> Skills { get; } = skills.ToDictionary(skill => skill.Id);
    }
}
