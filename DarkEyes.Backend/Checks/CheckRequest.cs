using System;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Checks;

/// <summary>A request for a check: what is attempted, by whom, on what.</summary>
public sealed record CheckRequest(CheckPurpose Purpose, EntityId Actor, EntityId? Target)
{
    public CheckPurpose Purpose { get; } = Purpose == default
        ? throw new ArgumentException("A check request needs a purpose.", nameof(Purpose))
        : Purpose;
}
