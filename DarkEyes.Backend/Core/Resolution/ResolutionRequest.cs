using System.Collections.Generic;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution
{
    public readonly struct ResolutionRequest(
        IStat subject,
        int challengeRating,
        IReadOnlyList<IPreRollModifier> preRoll,
        IReadOnlyList<IPostRollModifier> postRoll
        )
    {
        public readonly IStat Subject = subject;
        public readonly int ChallengeRating = challengeRating;
        public readonly IReadOnlyList<IPreRollModifier> PreRoll = preRoll;
        public readonly IReadOnlyList<IPostRollModifier> PostRoll = postRoll;
    }
}
