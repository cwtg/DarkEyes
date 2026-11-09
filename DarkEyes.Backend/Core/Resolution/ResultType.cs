namespace DarkEyes.Backend.Core.Resolution;

public enum ResultType : byte
{
    CriticalBlunder,
    Blunder,
    Failed,
    Success,
    CriticalSuccess,
    SuperCriticalSuccess,
}
