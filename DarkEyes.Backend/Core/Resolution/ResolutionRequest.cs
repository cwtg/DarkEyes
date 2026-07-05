using System.Collections.Generic;
using DarkEyes.Backend.Core.Modifier;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution
{
    public sealed record ResolutionRequest : IPreRollContext
    {
        public IStat? RelevantStat { get; set; }
        public IReadOnlyList<IModifier> Modifiers { get; set; } = [];
    }
}
