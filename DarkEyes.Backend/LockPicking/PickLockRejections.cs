using DarkEyes.Backend.Pipeline;

namespace DarkEyes.Backend.LockPicking;

/// <summary>The reasons why picking a lock can be refused.</summary>
public static class PickLockRejections
{
    public const string TooFarAwayCode = "pick_lock.too_far_away";

    public const string NotLockedCode = "pick_lock.not_locked";

    public const string DistanceArgument = "distance";

    public const string MaxArgument = "max";

    public static Rejection TooFarAway(int distance, int maxDistance) =>
        Rejection.Create(TooFarAwayCode, (DistanceArgument, distance), (MaxArgument, maxDistance));

    public static Rejection NotLocked() => Rejection.Create(NotLockedCode);
}
