using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.Containers;

/// <summary>The reasons why opening a container can be refused.</summary>
public static class OpenContainerRejections
{
    public const string TooFarAwayCode = "open_container.too_far_away";

    public const string AlreadyOpenCode = "open_container.already_open";

    public const string LockedCode = "open_container.locked";

    public const string DistanceArgument = "distance";

    public const string MaxArgument = "max";

    public static Rejection TooFarAway(int distance, int maxDistance) =>
        Rejection.Create(TooFarAwayCode, (DistanceArgument, distance), (MaxArgument, maxDistance));

    public static Rejection AlreadyOpen() => Rejection.Create(AlreadyOpenCode);

    public static Rejection Locked() => Rejection.Create(LockedCode);
}
