# Implementation plan 01: "A hero opens a chest"

Handoff for an implementation session. Agreed with the user on 2026-11-09.

## Read first

- `CLAUDE.md`: binding rules and collaboration conventions.
- `docs/architecture/architecture.md`: the authoritative target architecture. Especially relevant: §6.1 (the runtime view of exactly this use case), §8.1–8.3 (state, pipeline, commit result/patches), §8.8 (determinism rules), §8.9 (errors), ADR-010 (rule module).

## Approach (decided)

- **Use-case driven, true TDD.** Red → green → refactor, one small step at a time. Test intent through the public API (dispatch → commit result → mirror), not implementation details. Build only what the current test needs.
- **Strangler: build alongside, replace step by step.** The new core grows in new namespaces next to the old code. Once a use case is ported, delete the old handler and its tests. `main` must build and all tests must pass after every step.
- This story is the first part of the **walking skeleton** (see `decisions.md`): the following stories build on it until the result can be shown end to end in Unity.
- This use case is chosen because it forces out the most problematic parts of the current code (see below) while staying small. Movement (pathfinding, scheduler, footprints) is a later, separate use case.

## Steps

1. **The hero opens an adjacent, unlocked chest.** This creates the minimal core: immutable state root, entities with components keyed by stable name, dispatch of an action, one effect, one op, and a commit result with one `(event, patches)` step. A minimal projection plus a test mirror lets the test assert on what the frontend would see. Opening a container is generic gameplay, so the action, handler, component and event live in the backend (feature folder `Containers/`, ADR-010).
2. **The chest is too far away** → typed rejection (e.g. `TooFarAway(distance, max)`); state unchanged.
3. **The chest is already open** → typed rejection.
4. **The chest is locked; the hero picks the lock** → success depends on the DSA5 check. This introduces the rule-module assembly (`DarkEyes.Rules.Dsa5`): the backend defines the interface it needs for unlocking (shaped by this use case only), and the rule module implements it with the 3d20 lock-picking check, moved out of the current `Core/Resolution`. It also adds our own seeded PRNG (with streams) instead of `System.Random`. See §6.1.
5. **Picking the lock fails** → the chest stays locked; the event carries the roll.

Next use case afterwards (separate plan): taking an item from the chest into the party inventory. It reuses the parked inventory drafts on branch `wip/entitystore-refactor`.

Sketch of the first test. The API shape is not fixed; let the test drive it.

```csharp
[Fact]
public void Should_OpenChest_When_HeroIsAdjacentAndChestIsUnlocked()
{
    // Arrange
    var game = new ScenarioBuilder()
        .WithHero("hero.alrik", at: new GamePosition(0, 0))
        .WithChest("chest.cellar", at: new GamePosition(1, 0), locked: false)
        .Build();

    // Act
    var result = game.Dispatch(new OpenContainerAction("hero.alrik", "chest.cellar"));

    // Assert
    Assert.True(result.IsCommitted);
    Assert.IsType<ContainerOpened>(result.Steps.Single().Event);
    Assert.True(game.Mirror.Get<ContainerState>("chest.cellar").IsOpen);
}
```

## Problems in the current code this cleans up

| Current code | Replaced by |
|---|---|
| Mutable `Entity` with setters; `States`/components keyed by `Type`; `EntityStore` per location | Immutable state tree; components keyed by stable name in ordinal-sorted maps; one global entity store |
| Handlers write state directly; `GameOrchestrator` with an Rx `Subject` | Action → effect → op → commit; push via BCL `IObservable` (or nothing yet, if the tests don't need it) |
| `ErrorRaisedEvent` strings | Typed rejection codes with parameters |
| `SeededRandomSource` on `System.Random`; tests passing `(new Random() as IRandomSource)!` (always `null`) | Own PRNG, state-held streams, seeded tests |
| DSA code in the engine (`Core/Stats`, `Core/Modifiers`, `Core/Resolution`, `"lockpicking"` in `OpenContainerHandler`) | Rule-module assembly |
| `ChallengeRatingModifier` adds the challenge rating (higher = easier?) | A test against the DSA5 rule; flag the deviation to the user if it is one |

When step 5 is green, delete `OpenContainerHandler`, `OpenContainerCommand`, their events and `OpenContainerTests`, plus anything else that has become unused. Leave `MoveCharacter` and the old pipeline alone until movement is ported.

## Not in scope

Projection diffing beyond what the tests need; scheduler and time; movement and pathfinding; perception and knowledge; Yarn; persistence; AI; Unity code. Don't build ahead for them, but don't contradict the architecture either.

## Decisions the implementing session should raise with the user (not decided yet)

- Concrete public API shape: the dispatcher entry point, the commit result type, the rejection type; from step 4, how the rule module registers its unlock implementation and how `ScenarioBuilder` gets it.
- Where rule-module tests live (from step 4): the existing test project referencing both assemblies, or a separate `DarkEyes.Rules.Dsa5.Tests`.
- Which PRNG (PCG32 or xoshiro128\*\*) and how streams are derived from the master seed.
- How "adjacent" is measured before footprints exist (hex distance between positions is the obvious interim; footprints come with movement).
- Where `ScenarioBuilder` and the test mirror live (test project vs. a shared test-support library).

## Working rules (from the user)

- Undecided points are discussed with the user before implementing. Obvious consequences of decided principles are implemented and briefly reported.
- Commit only after the user agrees, or when committing is part of the task.
- Flag bugs, inconsistencies and DSA5 deviations you notice.
- The architecture doc holds only architecture. Implementation status, plans and branches don't go there.
