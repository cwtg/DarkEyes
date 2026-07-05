namespace DarkEyes.Backend.Core.Resolution
{
    public static class DiceRoll
    {
        public static int D6(IRandomSource random, int amount = 1) => RollDice(random, 6, amount);

        public static int D20(IRandomSource random, int amount = 1) => RollDice(random, 20, amount);

        private static int RollDice(IRandomSource random, int sides, int amount)
        {
            if (amount < 1) return 0;
            if (amount == 1) return random.Next(1, sides + 1);

            int result = 0;
            for (int i = 0; i < amount; i++) result += random.Next(1, sides + 1);

            return result;
        }
    }
}
