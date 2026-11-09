using System.Collections.Immutable;
using DarkEyes.Backend.Visibility;

namespace DarkEyes.Backend.Pipeline;

/// <summary>An event plus the patches caused by it.</summary>
public sealed record Step(IEvent Event, ImmutableArray<Patch> Patches);
