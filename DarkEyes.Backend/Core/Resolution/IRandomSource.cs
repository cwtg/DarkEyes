namespace DarkEyes.Backend.Core.Resolution;

public interface IRandomSource
{
    int Next();

    int Next(int max);

    int Next(int min, int max);
}
