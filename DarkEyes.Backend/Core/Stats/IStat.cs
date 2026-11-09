namespace DarkEyes.Backend.Core.Stats;

public interface IStat
{
    StatId Id { get; }

    int Value { get; }
}
