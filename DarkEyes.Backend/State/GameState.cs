using System;
using System.Collections.Immutable;
using DarkEyes.Backend.Randomness;

namespace DarkEyes.Backend.State;

/// <summary>The root of the immutable state tree. Only created and changed through its own members.</summary>
public sealed record GameState
{
    private GameState(ImmutableSortedDictionary<EntityId, Entity> entities, ulong? masterSeed, ImmutableSortedDictionary<RandomStreamId, Pcg32State> randomStreams)
    {
        Entities = entities;
        MasterSeed = masterSeed;
        RandomStreams = randomStreams;
    }

    public static GameState Empty { get; } = new(
        ImmutableSortedDictionary.Create<EntityId, Entity>(EntityId.Ordinal),
        masterSeed: null,
        ImmutableSortedDictionary.Create<RandomStreamId, Pcg32State>(RandomStreamId.Ordinal));

    public ImmutableSortedDictionary<EntityId, Entity> Entities { get; private init; }

    /// <summary>The seed every PRNG stream is derived from; unset until a new game sets it. Internal: whoever can read streams can predict rolls.</summary>
    internal ulong? MasterSeed { get; private init; }

    /// <summary>The streams used so far; a stream not in here starts from its derived seed.</summary>
    internal ImmutableSortedDictionary<RandomStreamId, Pcg32State> RandomStreams { get; private init; }

    public Entity GetEntity(EntityId id) => Entities[id];

    public GameState WithEntity(Entity entity) => this with { Entities = Entities.SetItem(entity.Id, entity) };

    /// <summary>A new game's seed. Only before any stream is used: changing it later would silently change the rolls to come.</summary>
    public GameState WithMasterSeed(ulong masterSeed)
    {
        if (!RandomStreams.IsEmpty)
        {
            throw new InvalidOperationException("The master seed can't change once a random stream has been used.");
        }

        return this with { MasterSeed = masterSeed };
    }

    internal Pcg32State GetRandomStream(RandomStreamId stream)
    {
        if (RandomStreams.TryGetValue(stream, out Pcg32State state))
        {
            return state;
        }

        ulong masterSeed = MasterSeed
            ?? throw new InvalidOperationException("No master seed is set; a new game must set one (WithMasterSeed) before anything rolls.");
        return StreamSeeds.Derive(masterSeed, stream);
    }

    internal GameState WithRandomStream(RandomStreamId stream, Pcg32State state) => this with { RandomStreams = RandomStreams.SetItem(stream, state) };
}
