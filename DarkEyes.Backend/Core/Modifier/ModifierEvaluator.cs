using System.Collections.Generic;
using DarkEyes.Backend.Core.Resolution;

namespace DarkEyes.Backend.Core.Modifier
{
    internal static class ModifierEvaluator
    {
        internal static int ApplyPreRoll(int baseValue, IReadOnlyList<IPreRollModifier> modifiers, in PreRollContext context)
        {
            List<IPreRollModifier> sortedList = [.. modifiers];
            sortedList.Sort((a, b) => a.Order.CompareTo(b.Order));

            int finalValue = baseValue;
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].Target != context.Subject) continue;

                if (modifiers[i].IsActive(context))
                    finalValue = modifiers[i].Apply(finalValue);
            }

            return finalValue;
        }

        internal static ResolutionResult ApplyPostRoll(ResolutionResult result, IReadOnlyList<IPostRollModifier> modifiers, in PostRollContext context)
        {
            // tbd
            return result;
        }
    }
}
