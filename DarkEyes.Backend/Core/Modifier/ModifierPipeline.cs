using System;
using System.Collections.Generic;
using System.Linq;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifier
{
    public static class ModifierPipeline
    {
        public static ResolutionRequest ApplyPreRoll(ResolutionRequest request)
        {
            List<IModifier<IPreRollContext>> suitableMods = FindSuitableModifiers<IPreRollContext>(request.Modifiers, request);

            IStat[] stats = [request.RelevantStat];
            if (request.RelevantStat is Skill skill)
            {
                _ = stats.Append(skill.Attribute1);
                _ = stats.Append(skill.Attribute2);
                _ = stats.Append(skill.Attribute3);
            }

            foreach (IStat stat in stats)
            {
                foreach (IModifier<IPreRollContext> mod in suitableMods)
                {
                    ApplyModifier(stat, mod, request);
                }
            }

            return request;
        }

        public static ResolutionResult ApplyPostRoll(ResolutionResult result)
        {
            // tbd
            return result;
        }

        private static List<IModifier<T>> FindSuitableModifiers<T>(IReadOnlyList<IModifier> modifiers, T context)
        {
            var suitableMods = new List<IModifier<T>>(modifiers.Count);

            foreach (IModifier mod in modifiers)
            {
                if (mod is IModifier<T> phaseMod && phaseMod.Condition.IsActive(context))
                    suitableMods.Add(phaseMod);
            }

            suitableMods.Sort((a, b) => a.Combination.CompareTo(b.Combination));
            return suitableMods;
        }

        private static void ApplyModifier<T>(IStat stat, IModifier<T> mod, T context)
        {
            if (stat.Id.Value != mod.AffectedStat.Value) return;

            if (mod.Combination == CombinationType.Additive)
            {
                stat.Value += mod.Value;
            }
            else if (mod.Combination == CombinationType.Multiplicative)
            {
                stat.Value = stat.Value * mod.Value;
            }
        }
    }
}
