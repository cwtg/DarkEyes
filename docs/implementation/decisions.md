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
- **Legacy code** that a story replaces gets a file-level `#pragma warning disable <ids> // Legacy ...` that points to the story. These go away with the old code. New code gets no exemptions.
- CI runs on `windows-latest` to match the development machine (CRLF line endings under enforced formatting).
