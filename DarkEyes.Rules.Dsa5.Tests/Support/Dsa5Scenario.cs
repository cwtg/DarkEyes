using DarkEyes.Backend.Containers;
using DarkEyes.Backend.LockPicking;
using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;
using DarkEyes.Rules.Dsa5.Characters;
using DarkEyes.Rules.Dsa5.Locks;

namespace DarkEyes.Rules.Dsa5.Tests.Support;

/// <summary>Builds game situations with DSA5 data and a store with the backend's gameplay and the DSA5 rules installed.</summary>
internal sealed class Dsa5Scenario
{
    public static readonly EntityId Hero = new("hero.alrik");
    public static readonly EntityId Chest = new("chest.cellar");
    public static readonly EntityId HardChest = new("chest.vault");

    private GameState _state = GameState.Empty;

    public static Attributes AllAttributes(int value) => new(value, value, value, value, value, value, value, value);

    public Dsa5Scenario WithHero(Attributes attributes, int lockpicking)
    {
        _state = _state.WithEntity(Entity.Create(Hero)
            .With(Placement.Key, new Placement(new GamePosition(0, 0, 0)))
            .With(Attributes.Key, attributes)
            .With(SkillRatings.Key, SkillRatings.None.With(Skills.Lockpicking.Id, lockpicking)));
        return this;
    }

    public Dsa5Scenario WithHeroWithoutRatings(Attributes attributes)
    {
        _state = _state.WithEntity(Entity.Create(Hero)
            .With(Placement.Key, new Placement(new GamePosition(0, 0, 0)))
            .With(Attributes.Key, attributes));
        return this;
    }

    public Dsa5Scenario WithHeroWithoutAttributes()
    {
        _state = _state.WithEntity(Entity.Create(Hero)
            .With(Placement.Key, new Placement(new GamePosition(0, 0, 0))));
        return this;
    }

    public Dsa5Scenario WithLockedChest(int checkModifier) => WithLockedChest(Chest, checkModifier);

    /// <summary>A further locked chest, also next to the hero (at the other side).</summary>
    public Dsa5Scenario WithLockedChest(EntityId id, int checkModifier)
    {
        _state = _state.WithEntity(LockedChest(id).With(LockModifier.Key, new LockModifier(checkModifier)));
        return this;
    }

    public Dsa5Scenario WithLockedChestWithoutModifier()
    {
        _state = _state.WithEntity(LockedChest(Chest));
        return this;
    }

    private static Entity LockedChest(EntityId id) => Entity.Create(id)
        .With(Placement.Key, new Placement(id == Chest ? new GamePosition(1, 0, 0) : new GamePosition(-1, 0, 0)))
        .With(ContainerState.Key, new ContainerState(IsOpen: false))
        .With(Lock.Key, new Lock(IsLocked: true));

    public GameState BuildState(ulong seed) => _state.WithMasterSeed(seed);

    public GameStore Build(ulong seed)
    {
        GameStoreBuilder builder = new GameStoreBuilder().AddContainers().AddLocks().AddLockPicking();
        return Dsa5Rules.Install(builder).Build(BuildState(seed));
    }
}
