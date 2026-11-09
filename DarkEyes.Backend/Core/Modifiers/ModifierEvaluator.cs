using System.Collections.Generic;

namespace DarkEyes.Backend.Core.Modifiers;

internal static class ModifierEvaluator
{
    internal static int ApplyModifiers(int baseValue, IReadOnlyList<IModifier> modifiers, in ModifierContext context)
    {
        int finalValue = baseValue;
        for (int i = 0; i < modifiers.Count; i++)
        {
            if (modifiers[i].Target != context.Subject)
            {
                continue;
            }

            if (modifiers[i].IsActive(context))
            {
                finalValue = modifiers[i].Apply(finalValue);
            }
        }

        return finalValue;
    }
}
