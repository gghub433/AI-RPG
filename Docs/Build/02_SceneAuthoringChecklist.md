# Scene Authoring Checklist (Editor-Required Steps)

The five scene files under `Assets/Game/Scenes/` currently contain only a single
empty root `GameObject` each (see `Docs/Build/00_Status.md`). This checklist is
what a developer opening the project in a real Unity Editor should do to turn them
into the "first playable version" described in the design brief. Hand-authoring
full scene hierarchies (prefabs, lighting, NavMesh bakes, Canvas/UI trees) as raw
YAML without the Editor's GUID management risks silently corrupt scenes, so this
work is intentionally left as an in-editor task rather than guessed at in text.

## Boot
- [ ] Add a `GameManager` component to `BootLoader`, mark `DontDestroyOnLoad` (handled in code already).
- [ ] Add a `SaveController`-less bootstrap step that checks for an autosave and
      transitions to MainMenu (`GameManager.LoadSceneAsync("MainMenu", GameState.MainMenu)`).

## MainMenu
- [ ] Build a `Canvas` (Screen Space - Overlay, matched to the project's reference
      resolution, e.g. 1920x1080 with a `CanvasScaler` set to Scale With Screen Size).
- [ ] Add buttons: New Game, Continue, Settings, Quit; wire to `MainMenuController`.
- [ ] Add a Settings panel bound to `GameSettings` fields (graphics preset dropdown
      → `QualitySettingsManager.ApplyPreset`; volume sliders → an `AudioManager`
      mixer, to be authored; text-scale slider → TMP `GlobalFontAsset` or a
      per-Canvas scale multiplier).

## Prologue (Arc I content)
- [ ] Build the Lower Gallery B greybox layout per `Docs/Story/Arcs/Arc01_DeathByBetrayal.md`.
- [ ] Place `CharacterStats` + `CombatController` + `PlayerController` on the Arden
      prefab (original-body variant, no Eclipse System UI active yet).
- [ ] Place `EnemyController` + `CharacterStats` + `StatusEffectController` on
      Hollow Wisp enemy prefabs, referencing an `EnemyArchetype` asset.
- [ ] Build the Scene 3 cutscene using Timeline (`com.unity.timeline`, already in
      `Packages/manifest.json`).
- [ ] Wire the Scene 4 System-panel reveal to `EclipseSystemHUD`.

## FirstRegion (Veyth Hollow, Arc II) / FirstDungeon (Aldenwatch Spire, Arc III)
- [ ] Follow the same pattern: greybox layout from the relevant Arc document,
      populate `QuestDefinition`/`DialogueDefinition`/`NPCDefinition` assets under
      `Assets/Game/Data/`, wire `InteractableObject` subclasses to world objects.
- [ ] Bake NavMesh for both scenes once geometry is final (required for
      `EnemyController`'s `NavMeshAgent` dependency).

## Cross-Cutting
- [ ] Create one `ItemDatabase`, one `QuestDatabase`, one `NPCDatabase` asset per
      project (not per scene) under `Assets/Game/Data/`, and reference them from
      the relevant manager components in each scene.
- [ ] Add all five scenes to **File → Build Settings → Scenes In Build**.
