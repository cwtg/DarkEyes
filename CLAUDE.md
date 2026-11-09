# DarkEyes

Hobby project: a single-player, party-based, tactical and strategic RPG set in Aventuria, using the rules of *Das Schwarze Auge* 5th edition (DSA5 / The Dark Eye). Release is possible later, so **DSA5-specific mechanics must stay separable** (trademark/licensing).

**The architecture is documented in [`docs/architecture/architecture.md`](docs/architecture/architecture.md) (arc42 + ADRs). It is authoritative.** Read the relevant section before working on an area. The rules below are the condensed, binding summary for day-to-day coding. When a discussion settles something new, update both.

## Solution layout

| Project | Purpose |
|---|---|
| `DarkEyes.Backend` | Engine and all game logic except DSA5-specific mechanics. netstandard2.1, C# `latest`, nullable enabled, no implicit usings. Unity-free. A post-build target copies DLLs into `DarkEyes.Unity/Assets/Plugins` (currently including System.Reactive, which is to be removed). |
| `DarkEyes.Backend.Tests` | xUnit, net9.0. `Unit/` and `Integration/` mirror the backend namespaces. |
| `DarkEyes.Rules.Dsa5` | Rule module: only the DSA5-specific mechanics, installed through one entry point. netstandard2.1, references the backend (never the reverse); copied into `DarkEyes.Unity/Assets/Plugins` like the backend. |
| `DarkEyes.Rules.Dsa5.Tests` | xUnit, net9.0. Rule tests with scripted rolls, integration tests through the store. |
| `DarkEyes.Unity` | Unity 6000.3, URP, new Input System, PC. Presentation only; no scripts yet. |
| *planned* | AI controller modules (folder first, `DarkEyes.AI.*` assemblies later); Unity assemblies `Presentation`, `Editor`, `Tests` (`Rules.Dsa5` later). |

Build/test: `dotnet build DarkEyes.sln`, `dotnet test DarkEyes.sln`.

## Binding rules (summary; details and reasons in the architecture doc)

**Ownership and flow**
- The backend owns all state and rules; Unity presents, collects input and sends **actions**. No game logic in Unity.
- **An action is one act as the rules define it**; the backend never adds acts the actor didn't choose. Its execution may span steps or time (movement cell by cell) and stays one act. Composition (walk-then-open, automation, auto-answers) is the frontend's or AI controller's job, with ordinary actions validated one by one.
- One **immutable state tree** (records + `System.Collections.Immutable`, no mutable members). Commits are final; no undo.
- Pipeline: **actions** (actors, rule-checked) → **effects** (decreed, invariant-enforcing) → **ops** (record-level writes). Player and AI enter at actions; scheduler, story, reactions and debug at effects. Authority is enforced by entry points. Shared preconditions are declared at registration.
- Handlers work on a transaction view; the transaction is applied atomically or discarded. Reactions run in a queue within the same run (depth limit); re-entrant dispatch is forbidden.
- **Player decisions mid-action:** `ctx.Decide(...)` + replay. An interrupted chain is stored as `PendingResolution { Root, Origin, Answers }` with `Root = Action | SchedulerEntry(id) | TriggerFiring(id, event) | RunNode(node)` and re-run from the committed state on each answer; steps are pushed provisionally. A pending scheduler chain leaves its entry due; the clock stays at the entry's time and the rest of the real-time chunk is dropped; the projection shows "decision pending, time stopped". Nothing else runs while a chain is pending. If an AI-controlled actor must decide, `Decide` calls its controller **inline** (no suspend, nothing stored; recomputed on re-runs).
- **Errors:** expected rejections return a `Reason`: a stable, namespaced reason code with named arguments (rejections by act, `pick_lock.*`; the same type says why a check is not possible, namespaced by the rule module, `dsa5.*`). The code is no localization key; the frontend maps codes to texts itself.
  - **Line:** the line between rejection and bug is the observer's projection. A target that is in the last projection sent but no longer valid in the truth (moved, gone) gives a rejection. Malformed input (an entity absent from that projection, a wrong target kind, a `Decide` answer that wasn't offered) is a caller bug and throws. This applies to AI controllers too.
  - **No leaks:** rejection arguments use only what the observer knows; the bare fact of a rejection is rule-visible. An impossible check is never a rejection but an outcome carrying the rule reason (`dsa5.attribute_too_low(attribute)`).
  - **Timing:** rejections are pure validation, only before the first check, effect, event or `Decide`; afterwards, failures are outcomes (events), and a late rejection is a handler bug.
  - Bugs throw and **fail hard** for every origin (for now).

**Frontend sync**
- Commit result = ordered `(event, patches)` steps. **Patches come from diffing projections** `Project(before, observer)` vs `Project(after, observer)`, never written by handlers or translated from ops. The diff compares against the **last projection actually sent** to that observer (self-healing for load, provisional steps, desyncs). `Project` returns the stored component instance unchanged when nothing is redacted and caches redacted views per instance; projected types containing collections get value equality (records compare collections by reference). Only **dirty entities** are diffed (touched by ops or with changed perception); derived values are recomputed every step. Integration tests can check: mirror after all patches == `Project(final state)`.
- Push via the BCL `IObservable<CommitResult>` (no System.Reactive in the backend), notified after the commit, **trampolined**.
- Events carry enough data to be presented without queries; they pass through the observer view. Every event type registers an observer-view policy (next to its presenter); unregistered events are **dropped** (fail closed). Redaction is mandatory; non-entity state is hidden by default; queries have no RNG access.
- The backend never considers animations. The frontend has a step player with registered presenters and two presentation regimes (clock-driven in real time, step-driven in rounds/dialogue).

**Determinism**
- Same state + seed + inputs = same result. All randomness through `IRandomSource`, backed by our **own PRNG**, with **separate streams per random consumer** (rule resolution, each AI controller, content, story; a consumer is a part whose draw count changes independently of the others) stored in the state. No `System.Random`, wall-clock time or `Guid.NewGuid()` in logic.
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
- **The backend is this game, not a generic engine.** The goal is only to make **trademarked mechanics replaceable**. Generic gameplay (containers, doors, inventory, movement, dialogue, common RPG concepts under generic names such as `LifePoints`, not `LeP`) lives in `DarkEyes.Backend`.
- **Rule module** (separate assembly, references the backend, never the reverse) holds only DSA5-specific mechanics: the 3d20 check, quality levels, formulas, tables, special abilities, spells, DSA terminology. **Checks are a backend concept:** an act requests a check by *purpose* (e.g. picking this lock) with actor and target; the rule module decides skill, attributes, modifiers (from the state), the roll, `Decide` choices such as rerolls, the check event and side consequences (blunders), and returns a generic result: succeeded (with degree) or failed, or **not possible** (with the rule reason). Its meaning for the outcome is the caller's decision. `Roll` never throws for impossibility. The check service also offers a side-effect-free **assessment** without dice (may it be attempted, else a rule reason; chance of success), used by previews and AI (not by validation). **"Not possible" is always a result, never a rejection**, for chosen and imposed checks alike; the caller turns it into an outcome (imposed checks usually treat it like failure; a dialogue check writes it into its variable). A chosen attempt costs what the rule module says (DSA5: at most a moment) and writes knowledge that this actor can't attempt this check on this target, so assessments stop offering it (no retry loops). For the frontend, arguments and chance use only what the observer knows. Act-specific rule interfaces only where an act needs more than a check (no speculative generic rule interfaces). The rule module plugs in through one registration entry point. Setting content only in content files.
- Check purposes live in one central catalog (`Checks/CheckPurposes`), not in the feature folders: it is the contract the rule module covers; the rule module tests that it maps every purpose.
- **AI modules** are swappable controllers: read-only, deterministic, own RNG stream (also for decision variation); assignment to actors is data. An AI **never observes the rule stream**: look-ahead uses the side-effect-free check assessments, or samples on state copies whose rule stream is replaced by one forked from the AI's stream (a copy carries the real rule stream, so real checks on it would reveal the next real rolls). They start as their own folder/namespace behind the controller interface and become separate assemblies once a second AI exists (same for `Unity.Rules.Dsa5` once real DSA UI exists). The rule module is a separate assembly **from the start**.
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
- `Core/Stats`, `Core/Modifiers`, `Core/Resolution`: the old DSA5 check engine. It is ported to the rule module (`DarkEyes.Rules.Dsa5`); the old copy is deleted in step 6 of plan 01.
- `Entities` (components keyed by `Type`), `Space` (hex grid, A*), `Inventory`, `Store`, `Time`, `Story`, `Persistence` (planned).
- An unfinished `EntityStore` Try-pattern refactor plus new inventory types (`ItemRef`, `ItemContainer`, `CurrencyWallet`, PickUpItem) is parked on branch `wip/entitystore-refactor` (doesn't build); `main` builds.

## Open design decisions — discuss before implementing

- Time model: trigger behaviour during narrative time jumps; normal-play time ratio.
- Before any release: handling of bugs in backend-originated commits (e.g. quarantine only the culprit in release builds).
- Rejection codes for the frontend (with the first Unity UI that shows rejections): codes as declared objects with their argument names, registered centrally (duplicate detection); type safety in Unity via a completeness test (every code mapped, placeholders match arguments) or generated accessors.
- Spike: System.Text.Json source generator in a Unity 6 IL2CPP build, weighed against handwritten codecs.
- Spike: world-space UI Toolkit in Unity 6.3 (fallback uGUI).
- Spike: Yarn VM in the backend: visit tracking stored as variables, compiled program shareable between VMs, detour detection, VM construction/reset cost, benchmark of re-running a large node, allocations under IL2CPP, Google.Protobuf dependency.
- Game-design save restrictions; scope of rule systems (decided incrementally); NPC/faction knowledge (with the first AI that needs it).

## Conventions

- `.editorconfig` is binding and **enforced in the build** (warnings are errors): Allman braces, braces always (also for one-line `if`s), `_camelCase` private fields, `s_` static fields, explicit types except when apparent, usings outside namespaces, **file-scoped namespaces**. Style questions are decided by .NET/C# standards, not by Unity's compiler limits. Shared build settings live in `Directory.Build.props`, package versions in `Directory.Packages.props`. No file headers. Suggest improvements when they're clearly better.
- **Types:** state, components, actions and events are `sealed record`s; small values (IDs, positions) are `readonly record struct`s; classes only for services/behaviour. Everything `sealed` unless designed for inheritance. Prefer primary constructors.
- **Collections in state** only as immutable collections (`ImmutableArray`, `ImmutableSortedDictionary`), wrapped in a value-equal type where value equality matters; never `IReadOnlyList`/`IReadOnlyDictionary` in records.
- **IDs:** `readonly record struct XyzId(string Value)` with validation; reject `default` explicitly where it matters.
- No public fields; no access modifiers on interface members.
- **Naming:** actions imperative with suffix (`OpenContainerAction`); events past tense without suffix (`ContainerOpened`); rejections are created only through per-act factories (`OpenContainerRejections.TooFarAway(...)`) with codes `act.snake_case` (`open_container.too_far_away`); content IDs lowercase, dot-separated (`hero.alrik`, `skill.lockpicking`), referenced through constants, never inline strings.
- **Feature folders** for new code (e.g. `Containers/` holds the action, handler, event and components), not folders per technical kind.
- Every code folder has a short `README.md`: a quick, high-level overview of the intent behind each group of files, in a few bullets. Not a file list and not a change log (no status, history or plans). Update it when the concepts in the folder change. Test folders that only mirror a backend folder need none.
- **One type per file**, named after the type (`ComponentKey{T}.cs` for generics). Exception: a closed hierarchy of small records (an abstract base and its cases) may share one file named after the base.
- Tests: xUnit, `Should_<Outcome>_When_<Condition>`, `// Arrange / // Act / // Assert`. Prefer **TDD**; test intent through the public API (integration tests via dispatch) over implementation details. Rule tests use a **scripted random source** (fixed rolls); real seeds only for determinism and golden tests.
- English for code, comments, docs and commit messages.

## Implementation workflow

- The user reviews every change before it is committed.
- Commit guideline: the changes of one commit can be summarized in one sentence, the solution builds, and all tests are green. This is a guideline, not an implementation goal; if it isn't possible for a commit, that's fine.
- Implementation decisions are settled when they come up. Architecture decisions are settled up front, before coding.
- General implementation decisions go into `docs/implementation/decisions.md`; story-specific ones go into that story's plan file.
- Before a session stops, add a short note to the current plan file: where things stand and what comes next.
