using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Modifiers
{
    public sealed class ChallengeRatingModifier(StatId target, int challengeRating) : IModifier
    {
        public StatId Target => target;
        public int Order => 100;
        private readonly int _value = challengeRating;

        public int Apply(int input) => input + _value;

        public bool IsActive(in ModifierContext context) => true;
    }
}
