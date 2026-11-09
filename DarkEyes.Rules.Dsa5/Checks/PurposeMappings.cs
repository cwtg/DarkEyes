using System.Collections.Generic;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.State;
using DarkEyes.Rules.Dsa5.Characters;
using DarkEyes.Rules.Dsa5.Locks;

namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>The rule module's answer to the backend's catalog of check purposes. Lookup only, never enumerated.</summary>
internal static class PurposeMappings
{
    private static readonly Dictionary<CheckPurpose, PurposeMapping> s_mappings = new()
    {
        [CheckPurposes.PickLock] = new PurposeMapping(Skills.Lockpicking, LockModifierOfTarget),
    };

    public static bool TryGet(CheckPurpose purpose, out PurposeMapping mapping) => s_mappings.TryGetValue(purpose, out mapping!);

    private static int LockModifierOfTarget(CheckRequest request, GameState state)
    {
        if (request.Target is not { } target)
        {
            return 0;
        }

        Entity entity = state.GetEntity(target);
        return entity.Has(LockModifier.Key) ? entity.Get(LockModifier.Key).CheckModifier : 0;
    }
}
