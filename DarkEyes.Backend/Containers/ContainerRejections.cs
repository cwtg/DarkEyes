using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Containers;

/// <summary>The reasons why a container action can be refused.</summary>
public static class ContainerRejections
{
    public const string TooFarAwayCode = "container.too_far_away";

    public const string AlreadyOpenCode = "container.already_open";

    public const string DistanceArgument = "distance";

    public const string MaxArgument = "max";

    public static Rejection TooFarAway(int distance, int maxDistance) =>
        Rejection.Create(TooFarAwayCode, (DistanceArgument, distance), (MaxArgument, maxDistance));

    public static Rejection AlreadyOpen() => Rejection.Create(AlreadyOpenCode);
}
