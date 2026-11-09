using System;
using DarkEyes.Backend.Randomness;

namespace DarkEyes.Backend.Pipeline;

/// <summary>Draws from a stream of the transaction's state; every draw is an op, so it is committed or discarded with the chain.</summary>
internal sealed class TransactionRandomSource(Transaction transaction, RandomStreamId stream) : IRandomSource
{
    private readonly Transaction _transaction = transaction;
    private readonly RandomStreamId _stream = stream;

    public int Next(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
        {
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), $"The range [{minInclusive}, {maxExclusive}) is empty.");
        }

        unchecked
        {
            // The width of any non-empty int range fits a uint; the wraparound maps it back correctly.
            uint bound = (uint)maxExclusive - (uint)minInclusive;
            (uint value, Pcg32State next) = Pcg32.NextBounded(_transaction.State.GetRandomStream(_stream), bound);
            _transaction.Apply(new SetRandomStreamOp(_stream, next));
            return (int)((uint)minInclusive + value);
        }
    }
}
