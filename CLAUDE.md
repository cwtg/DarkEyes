# DarkEyes

Hobby project: a single-player, party-based, tactical and strategic RPG set in Aventuria, using the rules of *Das Schwarze Auge* 5th edition (DSA5 / The Dark Eye). Release is possible later, so **DSA5-specific mechanics must stay separable** (trademark/licensing).

**The architecture is documented in [`docs/architecture/architecture.md`](docs/architecture/architecture.md) (arc42 + ADRs). It is authoritative.** Read the relevant section before working on an area. The rules below are the condensed, binding summary for day-to-day coding. When a discussion settles something new, update both.

## Solution layout

| Project | Purpose |
|---|---|
| `DarkEyes.Backend` | Engine and (for now) all game logic. netstandard2.1, C# `latest`, nullable enabled, no implicit usings. Unity-free. A post-build target copies DLLs into `DarkEyes.Unity/Assets/Plugins` (currently including System.Reactive, which is to be removed). |
| `DarkEyes.Backend.Tests` | xUnit, net9.0. `Unit/` and `Integration/` mirror the backend namespaces. |
| `DarkEyes.Unity` | Unity 6000.3, URP, new Input System, PC. Presentation only; no scripts yet. |
| *planned* | `DarkEyes.Rules.Dsa5` (rule module, own assembly from the start); AI controller modules (folder first, `DarkEyes.AI.*` assemblies later); Unity assemblies `Presentation`, `Editor`, `Tests` (`Rules.Dsa5` later). |

Build/test: `dotnet build DarkEyes.sln`, `dotnet test DarkEyes.sln`.

## Binding rules (summary; details and reasons in the architecture doc)

**Ownership and flow**
- The backend owns all state and rules; Unity presents, collects input and sends **actions**. No game logic in Unity.
- One **immutable state tree** (records + `System.Collections.Immutable`, no mutable members). Commits are final; no undo.
- Pipeline: **actions** (actors, rule-checked) → **effects** (decreed, invariant-enforcing) → **ops** (record-level writes). Player and AI enter at actions; scheduler, story, reactions and debug at effects. Authority is enforced by entry points. Shared preconditions are declared at registration.
- Handlers work on a transaction view; the transaction is applied atomically or discarded. Reactions run in a queue within the same run (depth limit); re-entrant dispatch is forbidden.
- **Player decisions mid-action:** `ctx.Decide(...)` + replay. An interrupted chain is stored as `PendingResolution { Root, Origin, Answers }` with `Root = Action | SchedulerEntry(id) | TriggerFiring(id, event) | RunNode(node)` and re-run from the committed state on each answer; steps are pushed provisionally. A pending scheduler chain leaves its entry due; the clock stays at the entry's time and the rest of the real-time chunk is dropped; the projection shows "decision pending, time stopped". Nothing else runs while a chain is pending. If an AI-controlled actor must decide, `Decide` calls its controller **inline** (no suspend, nothing stored; recomputed on re-runs).
- **Errors:** expected rejections return typed reason codes with parameters. Bugs throw and **fail hard** for every origin (for now).

**Frontend sync**
- Commit result = ordered `(event, patches)` steps. **Patches come from diffing projections** `Project(before, observer)` vs `Project(after, observer)`, never written by handlers or translated from ops. The diff compares against the **last projection actually sent** to that observer (self-healing for load, provisional steps, desyncs). `Project` returns the stored component instance unchanged when nothing is redacted and caches redacted views per instance; projected types containing collections get value equality (records compare collections by reference). Only **dirty entities** are diffed (touched by ops or with changed perception); derived values are recomputed every step. Integration tests can check: mirror after all patches == `Project(final state)`.
- Push via the BCL `IObservable<CommitResult>` (no System.Reactive in the backend), notified after the commit, **trampolined**.
- Events carry enough data to be presented without queries; they pass through the observer view. Every event type registers an observer-view policy (next to its presenter); unregistered events are **dropped** (fail closed). Redaction is mandatory; non-entity state is hidden by default; queries have no RNG access.
- The backend never considers animations. The frontend has a step player with registered presenters and two presentation regimes (clock-driven in real time, step-driven in rounds/dialogue).

**Determinism**
- Same state + seed + inputs = same result. All randomness through `IRandomSource`, backed by our **own PRNG**, with **separate streams per domain** (rules, each AI controller, content) stored in the state. No `System.Random`, wall-clock time or `Guid.NewGuid()` in logic.
- **Execution order is a rule decision**, with a stable engine fallback (priority → sequence number → ordinal ID). Components are keyed by **stable name**, not `Type`.
- Concretely: `ImmutableDictionary`/`HashSet` only for lookup, **never enumerate them** where order can matter; use `ImmutableSortedDictionary` with `StringComparer.Ordinal` (or integer keys) instead. Always pass ordinal comparers; never culture-default string comparison. Sort only with full tie-breaking (`OrderBy`/`ThenBy`). Enforced by **BannedApiAnalyzers** (`System.Random`, `DateTime.Now`, `Guid.NewGuid`, culture-default comparison, comparer-less `Sort`). Run a cross-runtime golden test (.NET 9 vs IL2CPP) early.
- **Integer maths** for anything that affects state; floats only for presentation.

**Time**
- Backend-owned integer game clock, time mode/scale, and a scheduler; it is the **only time mechanism** (no fine stepping). Movement = ongoing activity with one scheduler entry per cell; in rounds a move action steps cell by cell through the **same cell-entry hook**. **Only the active location runs movement**; other locations are frozen except for their triggers. **Fast time** (resting, waiting) is only available where the rules/content say it's safe (no per-cell movers nearby); overland travel is node-based anyway.
- **Triggers = When (game time or event pattern) + If (data condition) → Then (effect or Yarn node)**, for status durations, reactions and story alike. **General rules** (e.g. moving past an enemy provokes a free attack) are registered in code by the rule module; **stored triggers** in the state are only for concrete instances (this poison, a quest trigger), so saves never contain rule logic. Recurring effects re-arm themselves (their *Then* schedules the next trigger). A trigger only reacts to moments **after its creation**; a false *If* keeps it armed for its remaining *When* moments. How DSA durations ("3 KR", "until your next turn") map onto triggers is rule-module implementation (builders such as `Duration.Rounds(...)`); check DSA fidelity there.
- Rounds run in initiative order in the engine; the rule module supplies initiative values, ties, round duration and turn content.

**Entities, space, visibility**
- Entity = `Id + DefId + components` (pure data, keyed by stable name). Definitions are static content (JSON + schema), looked up live with per-component overrides. One global entity store; every entity is in exactly one place. Content IDs for authored entities, a state counter for spawns.
- Invisible hex grid (tentatively 3 hexes per pace, one constant) baked from Unity scenes; shared `HexLayout`; story uses named anchors only. Hexagonal footprints by radius, baked per-cell clearance. Preview path = execution path (`PreferredPath`). Outside rounds the party moves as **one group**: the leader follows the path, followers follow the leader's trail at slot offsets (no own pathfinding); party members don't block each other, NPCs do. In rounds every hero moves individually.
- The mirror holds the party's **knowledge**, stored as ordinary components/tags (`Discovered`, `Identified`, explored cells), including what is **currently perceived** (e.g. a `Perceived` set on the observer). Perception ≠ line of sight (DSA checks), so the rule module re-evaluates it after relevant events and writes it through effects; the projection never computes it. Per-component visibility policies.

**Modules**
- **Rule module** (separate assembly, references the engine, never the reverse) holds DSA5-specific mechanics. The engine may use common RPG concepts under generic names (`LifePoints`, not `LeP`). No speculative generic rule interfaces. Setting content only in content files.
- Common RPG concepts live **in the rule module by default** and move into the engine only when engine code actually needs them (e.g. initiative order for running rounds).
- **AI modules** are swappable controllers: read-only, deterministic, own RNG stream; assignment to actors is data. They start as their own folder/namespace behind the controller interface and become separate assemblies once a second AI exists (same for `Unity.Rules.Dsa5` once real DSA UI exists). The rule module is a separate assembly **from the start**.
- **Yarn** is the story/quest engine; its VM runs in the backend; variables live in the state; commands → effects, functions → queries; random built-ins replaced; the line-group **saliency strategy is always set explicitly** (deterministic or seeded; Yarn's default uses `System.Random`); a dialogue skill check is a command whose effect writes the result into a Yarn variable before the VM continues; **the VM is never state**: a fresh (or reset) VM per run, thrown away afterwards. A dialogue node runs as a replayable chain: choice points are `Decide`, answered by a choice or a rule-allowed action (dialogue is modal like a combat turn); the chain commits at node boundaries (`<<jump>>` without active detour) and at the end, with `ActiveDialogue { Node }` in the state between nodes. Silent trigger nodes run to completion inside their triggering chain; dialogue runs to the next choice point; game time stops during dialogue.

**Persistence and platform**
- Saves are JSON snapshots with a format version. **Until the formats are stable:** no migrations, and tests use C# scenario builders.
- **No runtime reflection** (IL2CPP stripping): explicit registration; compile-time source generators are fine.
- Only C#/BCL features available under netstandard2.1 / Unity (`IsExternalInit` polyfilled; no `required` members).
- Stay open to multiplayer (server-authoritative or lock-step); don't block it.
- **DSA5 fidelity:** as close as possible; discuss and document compromises; flag deviations.
- Performance is not a primary concern, but avoid gross waste (e.g. pathfinding must not allocate map-sized buffers per call).

## Current code (pre-architecture, to be migrated)

The code predates the agreed architecture; architecture doc §11.2 lists the gaps. Migration happens use case by use case (TDD, new code alongside the old, old code deleted once ported); plans for implementation sessions are in `docs/implementation/`. In short:
- Flow today: `IGameOrchestrator.Dispatch(ICommand)` → `CommandRouter` → `ICommandHandler<T>.Handle(command, HandlerContext)` → `context.Emit(IGameEvent)` → Rx `Events`. Handlers write mutable state directly; errors are `ErrorRaisedEvent` strings.
- `Core/Stats`, `Core/Modifiers`, `Core/Resolution`: DSA5 check engine (3d20 against three attributes, skill value as pool, quality level 1–6, crits/blunders). It belongs in the future rule module.
- `Entities` (components keyed by `Type`), `Space` (hex grid, A*), `Inventory`, `Store`, `Time`, `Story`, `Persistence` (planned).
- An unfinished `EntityStore` Try-pattern refactor plus new inventory types (`ItemRef`, `ItemContainer`, `CurrencyWallet`, PickUpItem) is parked on branch `wip/entitystore-refactor` (doesn't build); `main` builds.

## Open design decisions — discuss before implementing

- Time model: trigger behaviour during narrative time jumps; normal-play time ratio.
- Before any release: handling of bugs in backend-originated commits (e.g. quarantine only the culprit in release builds).
- Spike: System.Text.Json source generator in a Unity 6 IL2CPP build, weighed against handwritten codecs.
- Spike: world-space UI Toolkit in Unity 6.3 (fallback uGUI).
- Spike: Yarn VM in the backend: visit tracking stored as variables, compiled program shareable between VMs, detour detection, VM construction/reset cost, benchmark of re-running a large node, allocations under IL2CPP, Google.Protobuf dependency.
- Game-design save restrictions; scope of rule systems (decided incrementally); NPC/faction knowledge (with the first AI that needs it).

## Conventions

- `.editorconfig` is binding (Allman braces, `_camelCase` private fields, `s_` static fields, explicit types except when apparent, usings outside namespaces, block-scoped namespaces). Suggest improvements when they're clearly better.
- Prefer primary constructors, records for value-like data, and strongly-typed IDs (`readonly struct` with `IEquatable`).
- Tests: xUnit, `Should_<Outcome>_When_<Condition>`, `// Arrange / // Act / // Assert`. Prefer **TDD**; test intent through the public API (integration tests via dispatch) over implementation details. Seeded random sources for anything that rolls dice.
- English for code, comments, docs and commit messages.
