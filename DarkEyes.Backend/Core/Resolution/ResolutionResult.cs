using System;

namespace DarkEyes.Backend.Core.Resolution;

public readonly struct ResolutionResult(ResultType type, int qualityLevel)
{
    public ResultType Type { get; } = type;

    public int QualityLevel { get; } = qualityLevel < 0
    ? throw new ArgumentException("Quality level cannot be smaller 0.", nameof(qualityLevel))
    : qualityLevel;

    public override string ToString() => $"{nameof(Type)} with quality level {QualityLevel}";

    public bool IsSuccess => Type == ResultType.Success || Type == ResultType.CriticalSuccess || Type == ResultType.SuperCriticalSuccess;
}
