namespace DarkEyes.Backend.Randomness;

/// <summary>
/// PCG32 (XSH RR), our own PRNG: integer-only, identical on every runtime, pure functions over <see cref="Pcg32State"/>.
/// See https://www.pcg-random.org; matches the reference implementation (pcg-c-basic). All arithmetic wraps by design.
/// </summary>
internal static class Pcg32
{
    private const ulong Multiplier = 6364136223846793005UL;

    /// <summary>Seeds a stream like <c>pcg32_srandom_r(initialState, sequence)</c>.</summary>
    public static Pcg32State Seed(ulong initialState, ulong sequence)
    {
        unchecked
        {
            Pcg32State state = new(0, (sequence << 1) | 1UL);
            state = Next(state).State;
            state = state with { State = state.State + initialState };
            return Next(state).State;
        }
    }

    public static (uint Value, Pcg32State State) Next(Pcg32State state)
    {
        unchecked
        {
            ulong old = state.State;
            Pcg32State next = state with { State = (old * Multiplier) + state.Increment };
            uint xorShifted = (uint)(((old >> 18) ^ old) >> 27);
            int rotation = (int)(old >> 59);
            uint value = (xorShifted >> rotation) | (xorShifted << ((-rotation) & 31));
            return (value, next);
        }
    }

    /// <summary>A value in [0, bound) without modulo bias (rejection sampling, like <c>pcg32_boundedrand_r</c>).</summary>
    public static (uint Value, Pcg32State State) NextBounded(Pcg32State state, uint bound)
    {
        unchecked
        {
            uint threshold = (0u - bound) % bound;
            while (true)
            {
                (uint value, Pcg32State next) = Next(state);
                state = next;
                if (value >= threshold)
                {
                    return (value % bound, state);
                }
            }
        }
    }
}
