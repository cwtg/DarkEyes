using System.Collections.Immutable;

namespace DarkEyes.Backend.Pipeline;

/// <summary>The ordered steps of one commit.</summary>
public sealed record CommitResult(ImmutableArray<Step> Steps);
