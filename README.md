# Rebirth: Eclipse System

A dark-fantasy anime 3D action-RPG for Android, built in Unity 6 (URP). Arden Vale,
betrayed and left to die by his own expedition commander, is pulled into the
Eclipse System — an ancient soul-continuity failsafe — and reborn in a new world
inside the body of a boy everyone has already written off as powerless.

**Start here:** `Docs/Build/00_Status.md` — an honest, itemized account of what in
this repository is implemented and verified vs. designed-but-not-yet-built. This
README covers setup/run/build; that file covers what actually works.

## Requirements

- Unity **6000.0.32f1** (or nearest 6000 LTS point release) with the **Android
  Build Support** module (OpenJDK + Android SDK/NDK).
- Git (for cloning; large binary art/audio assets are expected to use Git LFS once
  real assets are added — not yet needed with the current placeholder-free,
  code-and-design-only content).

> No Unity Editor, .NET SDK, or Android SDK was available in the environment this
> project was authored in. The C# was written and reviewed carefully but has not
> been compiled by a real Unity Editor yet — see `Docs/Build/00_Status.md` before
> assuming anything beyond "the source exists and reads correctly."

## Project Layout

```
Assets/Game/
  Art/ Audio/                    — integration points, no placeholder primitives shipped
  Data/                          — ScriptableObject data assets (to be authored in-editor)
  Prefabs/ Scenes/               — Boot, MainMenu, Prologue, FirstRegion, FirstDungeon
  Scripts/
    Core/        — GameManager, GameEvents, QualitySettingsManager
    Character/   — StatBlock/DerivedStats, CharacterStats, PlayerController
    Combat/      — DamageCalculator, CombatController, EnemyController, BossController, StatusEffect
    Progression/ — LevelingSystem, ClassDefinition, SkillTreeDefinition, EclipseSystemModel
    Inventory/   — ItemDefinition, InventorySystem, EquipmentManager, ItemDatabase
    Quests/      — QuestDefinition, QuestManager, QuestDatabase
    Dialogue/    — DialogueDefinition, DialogueRunner
    Relationships/ — NPCDefinition, RelationshipManager
    Abyss/       — AbyssAbsorptionSystem (the signature unique ability)
    World/       — RegionDefinition, RegionProgressTracker, InteractableObject
    SaveSystem/  — SaveData, SaveManager (JSON, multi-slot, backup/recovery), SaveController
    UI/          — EclipseSystemHUD, SystemNotificationQueue, MainMenuController
  Tests/EditMode/  — 23 NUnit tests covering the systems above
Docs/
  Story/     — full narrative design: overview + all 12 arcs
  Characters/ — 20+ NPC profiles
  Factions/  — 7-faction design
  Design/    — classes, regions, items, long-term progression plan
  Build/     — status report, Android build guide, scene-authoring checklist
```

## Running / Opening the Project

1. Install Unity 6000.0.32f1 via Unity Hub with Android Build Support.
2. Unity Hub → **Add** → point at this repository's root folder.
3. Open the project. First import will take a while (generates `Library/`).
4. Expect to need to create four `UniversalRenderPipelineAsset`s (Low/Medium/
   High/Ultra) and wire them into a `QualitySettingsManager` instance — see
   `Docs/Build/01_AndroidBuildGuide.md` §4 — since binary ScriptableObject assets
   like these aren't practical to hand-author outside the Editor.
5. Work through `Docs/Build/02_SceneAuthoringChecklist.md` to populate the
   currently-empty scene files with actual hierarchies, following the Arc I–III
   design docs for the first playable slice.

## Running Tests

Once open in the Editor: **Window → General → Test Runner → EditMode → Run All**.
The suite lives in `Assets/Game/Tests/EditMode/` and exercises leveling math,
damage calculation, inventory/equipment duplication guards, quest objective
gating, Abyss absorption rules, and a real save/load round trip against the
filesystem (using a reserved test slot index that never collides with player
save slots).

## Building an Android APK

See `Docs/Build/01_AndroidBuildGuide.md` for the full Player Settings table and
step-by-step build/verify procedure (including the `adb install`/`adb shell am
start` checks to actually confirm a build runs, rather than assuming it does).

## Design Philosophy (from the brief, kept here as a standing reference)

- Power grows from real, earned progress — no fake reward animations, no numbers
  that go up without a real system behind them (every `GameEvents` notification
  traces to an actual state change; see `UI/SystemNotificationQueue.cs`).
- No artificial grind gates (no energy/stamina systems blocking play), no
  pay-to-skip mechanics planned.
- Boss fights get individual multi-phase logic (`BossController`), not the shared
  trash-mob AI (`EnemyController`) scaled up.
- Non-stackable item duplication is structurally prevented (unique instance IDs),
  not just discouraged — covered by `InventorySystemTests`.
- Romance content is adult-only by a hard code-level gate
  (`NPCDefinition.isAdult`), never bypassable by dialogue or player choice.

## Known Gaps

Tracked in full in `Docs/Build/00_Status.md` — in short: no art/audio assets, no
populated scene hierarchies yet, no compiled/verified build, no APK produced in
this session (the tooling to do so was not present in this environment).
