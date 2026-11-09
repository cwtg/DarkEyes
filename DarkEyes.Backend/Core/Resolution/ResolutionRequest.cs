#pragma warning disable CA1051 // Legacy code, replaced or moved to the rule module in story 01 (docs/implementation/01-open-chest.md).

using System.Collections.Generic;
using DarkEyes.Backend.Core.Modifiers;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution;

public readonly struct ResolutionRequest(
    IStat subject,
    IReadOnlyList<IModifier> modifiers
    )
{
    public readonly IStat Subject = subject;
    public readonly IReadOnlyList<IModifier> Modifiers = modifiers;
}
