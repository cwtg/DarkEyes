using System;

namespace DarkEyes.Backend.Core.Stats
{
    public class Skill(StatId id, int skillValue, Attribute attribute1, Attribute attribute2, Attribute attribute3) : IStat
    {
        public StatId Id { get; } = id;
        public int Value { get; } = skillValue;

        public Attribute Attribute1 { get; } = attribute1;
        public Attribute Attribute2 { get; } = attribute2;
        public Attribute Attribute3 { get; } = attribute3;
    }
}
