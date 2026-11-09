# General implementation decisions

Decisions that apply to more than one story. Story-specific decisions live in the story's plan file; architecture decisions live in `docs/architecture/architecture.md`; coding conventions live in `CLAUDE.md`.

## 2026-11-09: Development practices

Agreed with the user before starting story 01.

**Wanted now**

- **CI:** a GitHub Actions workflow runs `dotnet build` and `dotnet test` on every push. Part of the tooling commit before story 01. The cross-runtime golden test (.NET 9 vs. IL2CPP) joins it later.
- **Walking skeleton:** an end-to-end slice from the backend into Unity, built up story by story. Story 01 (open a chest) is its first part; the following stories lead up to showing the result in Unity.
- **Headless console:** a small console app that loads a scenario, takes actions as text and prints events and patches, so the backend can be played and debugged without Unity.
- **Replays as bug reports:** start state + input log reproduce a run exactly (follows from the determinism rules); a failing replay becomes a regression test.
- **No long-lived branches:** work on `main` or on branches that live hours or days. Unfinished ideas go into plan files as notes, not into parked branches. `wip/entitystore-refactor` is mined for the inventory story and then deleted.

**Adopted, applied when they become relevant**

- **Resumable session ends:** before a session stops, the current plan file gets a short note on where things stand and what comes next.
- **Time-boxing:** tooling, spikes and refactorings get a time budget up front; keep producing visible progress regularly.
- **Content validation as tests:** once JSON definitions exist, a test checks every content file against its schema and all referenced IDs.
- **Property-based tests** (e.g. FsCheck or CsCheck) where invariants matter: hex math, the PRNG, "mirror after all patches == projection". Used selectively.
- **Dependabot** for NuGet packages.
- **Unity hygiene:** asset serialization "Force Text", visible meta files, Unity Smart Merge registered in git (LFS via `.gitattributes` is already set up).

## 2026-11-09: Build enforcement

- Shared settings in `Directory.Build.props` (warnings are errors, code style enforced in the build, `AnalysisMode=Recommended`), package versions in `Directory.Packages.props`, SDK pinned in `global.json`. `DarkEyes.Unity` has empty `Directory.*` files so these don't leak into Unity's generated projects.
- `BannedSymbols.txt` (BannedApiAnalyzers) applies to game code, not to test projects. Ordering without a comparer (`OrderBy`/`ThenBy`, parameterless `Sort`) is banned too, also for `int` keys, because default string ordering is culture-sensitive. Every overload that builds a sorted collection with the default comparer is banned as well (`ImmutableSortedDictionary`/`ImmutableSortedSet`: `Empty`, `Create`, `CreateRange`, `CreateBuilder`, `ToImmutableSorted…` without comparer; `SortedSet`/`SortedDictionary`/`SortedList` constructors without comparer). An explicit `null` comparer can't be caught by the analyzer. Culture-dependent string handling is caught by CA1304/CA1305/CA1307/CA1309/CA1310.
- Turned off: CA1716 (keywords of other .NET languages, irrelevant here); CA1707 in test projects (test names use underscores).
- **Legacy code** gets a file-level `#pragma warning disable <ids> // Legacy ...` that points to the story replacing it, or to architecture §11.2 while no story is planned for it yet. These go away with the old code. New code gets no exemptions, except a commented, targeted `#pragma` where an analyzer rule doesn't fit a deliberate construct (e.g. the empty `IsExternalInit` polyfill).
- **SonarAnalyzer.CSharp** runs in the build for all projects (the same rules SonarLint shows in the IDE), enforced like the other analyzers.
- CI runs on `windows-latest` to match the development machine (CRLF line endings under enforced formatting).

## 2026-11-09: Component access by key object

- Each component type declares its stable name once, as a static key: `ContainerState.Key = new ComponentKey<ContainerState>("container")`. Access goes through the key (`entity.Get(ContainerState.Key)`); there is no type-to-name registry.
- **Why:** type and name can't drift apart, no runtime lookup can fail for an unregistered type, and the key is the natural place for the codec and visibility policy later.
- A key is a value (name + type), not bound 1:1 to a type: it can be defined outside the type, and one type can have several keys (e.g. tags). Once codecs or tools need a central list, modules register their **keys** in a registry (which also detects name collisions between modules); the registry never creates its own type-to-name mapping. Extensions stay additive; rebinding an existing name to another type is deliberately not supported (it would break saves).

## 2026-11-09: Idempotent effects

- An effect whose target state already holds does nothing (no op, no event), e.g. `UnlockEffect` on an unlocked lock or `OpenContainerEffect` on an open container.
- **Why:** effects are decreed and can't be refused (§8.2), and a rejection there would be late (§8.9); decreeing callers such as the story need no check first.

## 2026-11-09: PRNG and stream derivation

- **PCG32** (XSH RR, as in the reference implementation): 64-bit state + increment, integer-only, reference output as a golden test. Bounded values by rejection sampling (no modulo bias).
- **Stream seeds:** `hash = FNV-1a-64(stream ID, UTF-8)`; `PCG32.Seed(initialState: SplitMix64(masterSeed ^ hash), sequence: SplitMix64(hash))`. Streams are created on first use; the state holds the master seed and the streams used so far, so adding a random consumer needs no migration and shifts nothing.
- **Every draw is an op** in the transaction: discarded with the chain, and re-runs (`Decide`) reproduce the same rolls. The master seed can't change once a stream has been used.
- Stream IDs live in one catalog (`Randomness/RandomStreamIds`).
- Stream state (master seed, streams, PCG32) is internal to the backend: whoever can read it can predict rolls. Game code, the rule module and AIs draw only through `IRandomSource`. The seed derivation is pinned by a golden test, since changing it would change every new game and every stream derived later from old saves.
- **No default seed:** a new game must set the master seed (entropy comes from outside the backend: clock and `Guid` are banned in logic); rolling without one throws, so a forgotten seed can't ship identical dice in every game. Test scenarios default to seed 0.
