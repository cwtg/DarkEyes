# Implementation plan 01: "A hero opens a chest"

Handoff for an implementation session. Agreed with the user on 2026-11-09.

## Read first

- `CLAUDE.md`: binding rules and collaboration conventions.
- `docs/architecture/architecture.md`: the authoritative target architecture. Especially relevant: §8.1–8.3 (state, pipeline, commit result/patches), §8.8 (determinism rules), §8.9 (errors and rejections), ADR-010 (rule module).

## Approach (decided)

- **Use-case driven, true TDD.** Red → green → refactor, one small step at a time. Test intent through the public API (dispatch → commit result → mirror), not implementation details. Build only what the current test needs.
- **Strangler: build alongside, replace step by step.** The new core grows in new namespaces next to the old code. Once a use case is ported, delete the old handler and its tests. `main` must build and all tests must pass after every step.
- This story is the first part of the **walking skeleton** (see `decisions.md`): the following stories build on it until the result can be shown end to end in Unity.
- This use case is chosen because it forces out the most problematic parts of the current code (see below) while staying small. Movement (pathfinding, scheduler, footprints) is a later, separate use case.

## Steps

1. **The hero opens an adjacent, unlocked chest.** This creates the minimal core: immutable state root, entities with components keyed by stable name, dispatch of an action, one effect, one op, and a commit result with one `(event, patches)` step. A minimal projection plus a test mirror lets the test assert on what the frontend would see. Opening a container is generic gameplay, so the action, handler, component and event live in the backend (feature folder `Containers/`, ADR-010).
2. **The chest is too far away** → rejection `container.too_far_away(distance, max)`; state unchanged.
3. **The chest is already open** → rejection.
4. **The chest is locked** → "open" gives the rejection `container.locked`.
5. **The hero picks the lock** (`PickLockAction`) → success depends on the DSA5 check; the chest is unlocked, not opened (opening is a separate action, see §8.2). This introduces the rule-module assembly (`DarkEyes.Rules.Dsa5`): the backend defines the interface it needs for unlocking (shaped by this use case only), and the rule module implements it with the 3d20 lock-picking check, moved out of the current `Core/Resolution`. It also adds our own seeded PRNG (with streams) instead of `System.Random`.
6. **Picking the lock fails** → the chest stays locked; the event carries the roll.

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
| `ErrorRaisedEvent` strings | Rejections: reason codes with named arguments |
| `SeededRandomSource` on `System.Random`; tests passing `(new Random() as IRandomSource)!` (always `null`) | Own PRNG, state-held streams, seeded tests |
| DSA code in the engine (`Core/Stats`, `Core/Modifiers`, `Core/Resolution`, `"lockpicking"` in `OpenContainerHandler`) | Rule-module assembly |
| `ChallengeRatingModifier` adds the challenge rating (higher = easier?) | A test against the DSA5 rule; flag the deviation to the user if it is one |

When step 6 is green, delete `OpenContainerHandler`, `OpenContainerCommand`, their events and `OpenContainerTests`, plus anything else that has become unused. Leave `MoveCharacter` and the old pipeline alone until movement is ported.

## Not in scope

Projection diffing beyond what the tests need; scheduler and time; movement and pathfinding; perception and knowledge; Yarn; persistence; AI; Unity code. Don't build ahead for them, but don't contradict the architecture either.

## Decisions made during implementation

- **Public API (step 1):** `GameStore` (built by `GameStoreBuilder`, gameplay registered via extension methods such as `AddContainers()`); `Dispatch(IAction)` is the player entry point and returns `DispatchResult` = `Committed(CommitResult)` (`Rejected` follows in step 2); `CommitResult.Steps` are `Step(Event, Patches)`; `Connect()` returns the full projection as patches. No `IObservable` push yet.
- **Pipeline (step 1):** actions and effects are different kinds and have separate routers (`ActionRouter`, `EffectRouter`), which are pure, independent lookups. A per-dispatch `Chain` (the architecture's term for a root and everything that follows from it) holds the transaction and both routers and runs the root; `ActionContext.Submit` goes through it. Per-run behaviour belongs to the chain: later the reaction queue and depth limit, roots other than actions (scheduler entry, trigger firing) and replay. Preconditions and rejections attach to action handling. Action handlers get an `ActionContext` (read, submit effects, emit events) and cannot write; effect handlers get an `EffectContext` (read, write ops, emit events). The `Transaction` is only the working copy. `Emit` closes a step: the event plus the patches of the ops since the previous event. Ops after the last event are a bug and throw.
- **State encapsulation (step 1, from review):** `Entity` and `GameState` are non-positional records with private constructors and private `init`, so outside code can't use `with` or pass its own maps; factories and `With` are the only way in, which keeps the ordinal comparers and the key type check. `Entity.Components` is internal. `GameStore.State` is internal (the frontend only sees projections); tests reach it via `InternalsVisibleTo`. `Patch` is closed (`private protected`). A `GameStoreBuilder` builds exactly one store.
- **Test support (step 1):** `ScenarioBuilder`, `TestMirror` and `TestIds` live in `DarkEyes.Backend.Tests/Support/`. Whether they move into a shared library is decided in step 5.
- **Component access:** by key object, see `decisions.md`.
- **Rejections (step 2):** one `Rejection` type (code + named arguments), not one type per reason: there will be many reasons, and most are only displayed. Codes are `feature.snake_case`, created only through per-feature factories (`ContainerRejections`), which keep code and arguments consistent. The code is no localization key; Unity maps codes to texts. `ToString()` gives a readable form for logs and tests instead of a hand-written message. Action handlers return `Rejection?`; on a rejection the whole chain is discarded (state and last projection sent unchanged) and `Dispatch` returns `Rejected(Rejection)`.
- **Rejections (step 2), addendum from the review:** arguments are restricted to `int`, `bool`, `string`, `EntityId`; argument names are constants next to the factory; `Rejection` has explicit value equality over code and arguments. A rejection after a submitted effect or emitted event throws (`Chain`); `GameStore` alone decides about the commit.
- **Malformed input (step 2):** a target without `ContainerState` throws `ArgumentException` in the action handler. An unknown entity ID throws from `GameState.GetEntity`.
- **Already open (step 3):** rejection `container.already_open` without arguments; checked before the range, so a far-away open chest reports "already open" (walking there would be pointless).
- **Reach (step 2):** hex distance between the `Placement` positions, at most 1 (neighbouring hex), until footprints exist. Known gap: `GamePosition.DistanceTo` ignores the level, so a container directly above or below counts as adjacent; fixed with levels in the movement story.
- **Sync invariant (step 2):** `TestMirror.AssertMatches(store)` checks mirror == projection of the current state; integration tests call it.

## Decisions the implementing session should raise with the user (not decided yet)

- From step 5, how the rule module registers its unlock implementation and how `ScenarioBuilder` gets it.
- Where rule-module tests live (from step 5): the existing test project referencing both assemblies, or a separate `DarkEyes.Rules.Dsa5.Tests`.
- Which PRNG (PCG32 or xoshiro128\*\*) and how streams are derived from the master seed.
- How "adjacent" is measured before footprints exist (hex distance between positions is the obvious interim; footprints come with movement).

## Working rules (from the user)

- Undecided points are discussed with the user before implementing. Obvious consequences of decided principles are implemented and briefly reported.
- Commit only after the user agrees, or when committing is part of the task.
- Flag bugs, inconsistencies and DSA5 deviations you notice.
- The architecture doc holds only architecture. Implementation status, plans and branches don't go there.

## Status

- **2026-11-09:** steps 1–3 are green (`Integration/Containers/OpenContainerTests.cs`). Not built yet, by design: observer-view policies for events, component visibility policies. The projection **throws** for an entity disappearing and for a component being added to or removed from a visible entity (no patch kinds for these yet); steps 4–5 may need "component added" for lock state.
- **Deferred (line = projection, §8.9):** today the projection equals the truth, so the range check against the true positions is equivalent. Once perception or remembered knowledge exists, validation must be measured against the observer's last projection (existence and remembered position). That also covers "no leaks".
- **Deferred from the review of step 1:**
  - **Unity dependencies:** `System.Collections.Immutable` 10.0.11 pulls in `System.Memory` 4.6.3 and `System.Runtime.CompilerServices.Unsafe` 6.1.2; only Immutable is copied to `Assets/Plugins`. Check in the Unity editor (dependency spike, at the latest with the walking skeleton): copy the dependencies or pick versions matching what Unity ships.
  - **Value equality of collections in records** (`Step.Patches`, `CommitResult.Steps`, `EntityAppeared.Components`, entity maps): decide when replay checks, golden tests or the mirror == projection comparison need it.
  - **Re-entrancy guard** on `GameStore.Dispatch`: add together with the `IObservable` push (the first way to re-enter), with a test.
- Next: step 4 (locked chest → rejection `container.locked`).
