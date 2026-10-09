# 3D Model Pipeline — Tripo3D

The game's 3D models are generated with **Tripo3D** (https://studio.tripo3d.ai) and
dropped into the project through a key-based pipeline so the gameplay code never
references a model by file path — only by a stable **model key**.

## ⚠️ Important: generation cannot run from the cloud session

The Tripo API host (`api.tripo3d.ai`) is **blocked by this cloud environment's egress
network policy** (the proxy returns a 403 policy denial on connect). So
`Tools/tripo_generate.py` **cannot run from inside a Claude cloud session** — it must
be run from your own machine (or CI) where Tripo isn't firewalled, OR you generate on
the Tripo web UI and drop the files in by hand.

To allow it from a cloud session instead, add `api.tripo3d.ai` under the environment's
**Network access → Allowed domains** (keep "Allow package managers" ticked), or raise
the access level. Steps: https://code.claude.com/docs/en/cloud-environments#network-access

## API key handling

- The script reads the key from the `TRIPO_API_KEY` environment variable **only**. Never
  hardcode it, never commit it, never put it in the manifest or a tracked file.
- `export TRIPO_API_KEY=tsk_...` before running. If a key was ever pasted into a chat or
  shared, rotate it in the Tripo console afterward.

## The three pieces

1. **Manifest** — `Assets/Game/Art/model_manifest.json`. One entry per model: its `key`
   (what the game references), a Tripo text prompt, and a `targetPath` where the GLB lands.
   58 models enumerated (Arden's 4 stages, 20-name cast, armor sets, helmets, 7 crystals,
   enemies, bosses, props).
2. **Generator** — `Tools/tripo_generate.py`. Reads the manifest, submits text-to-3D
   tasks, polls, downloads each GLB to its `targetPath`, writes a `.tripo.json` provenance
   sidecar (task id + prompt; never the signed URL or the key).
3. **Unity side** — an Editor import post-processor + a runtime registry that turn dropped
   GLBs into usable, key-addressable prefabs.

## Workflow A — API (batch, from your machine)

```bash
export TRIPO_API_KEY=tsk_xxx
# preview without spending credits or calling the API:
python3 Tools/tripo_generate.py --dry-run
# generate a cheap proof first:
python3 Tools/tripo_generate.py --only weapon_voidrend crystal_eclipse_core
# then a category, or everything missing:
python3 Tools/tripo_generate.py --category crystal
python3 Tools/tripo_generate.py
```

Flags: `--only <keys...>`, `--category <cat>`, `--force` (regen existing), `--dry-run`.
It skips any model whose `targetPath` already exists (unless `--force`), so re-runs are
cheap and resumable.

**Verify the API shape first.** The script's base URL, endpoint paths, model version and
output-field path are configurable constants at the top (and via `TRIPO_API_BASE`,
`TRIPO_CREATE_PATH`, `TRIPO_TASK_PATH`, `TRIPO_MODEL_VERSION` env vars). They are set to
Tripo's documented async text-to-3D shape, but the exact version/field names differ
between plans — confirm against https://developers.tripo3d.com before a full run, and do a
`--only` one-model test to validate the round trip.

## Workflow B — Web UI (manual)

1. On https://studio.tripo3d.ai, generate each model using the prompt from the manifest.
2. Export **GLB** (enable quad/FBX only if you need quad topology for retopo).
3. Save the file to the manifest `targetPath` (create the folders as needed).

## Unity import (automatic)

- `Assets/Game/Scripts/Editor/TripoModelPostprocessor.cs` runs on import for anything under
  `Assets/Game/Art/Models/`: medium mesh compression, read/write off, humanoid rig for
  Characters/Enemies/Bosses (so Mecanim retargeting works), blendshapes only for cast,
  and ASTC 1024 mip-mapped textures for Android. It only touches assets in that folder.
- Make a **prefab** from each imported model (add the gameplay components it needs:
  `CharacterStats`/`EnemyController` for actors, colliders, etc.).
- Register each prefab in a **`ModelRegistry`** asset
  (`Assets/Game/Scripts/World/ModelRegistry.cs`): map the manifest `key` →  the prefab.

## How the game resolves a model

Gameplay refers to models only by key:
- `HeroStage.armorSetId` / `weaponId` → `HeroStageVisualBinder` resolves them via
  `ModelRegistry` and swaps Arden's body/weapon when his stage changes.
- `EnemyArchetype.enemyId`, item/crystal keys, etc. resolve the same way.

With an **empty or partial `ModelRegistry`, the game still runs** — `Resolve()` returns
null and callers keep their placeholder, so you can wire models in incrementally rather
than needing all 58 before anything works.

## Rigging note

Tripo text-to-3D returns a static mesh. Characters/enemies/bosses (`"rig": true` in the
manifest) need a skeleton for animation — use Tripo's rigging step, Unity's Humanoid
auto-rig on import (the post-processor sets `animationType = Human` for those folders), or
an external rig (Mixamo/Blender). The generator downloads the static mesh; rigging is a
deliberate separate step.

## Licensing

Confirm your Tripo plan permits commercial game use and redistribution of generated
assets **before shipping**. Keep the `.tripo.json` provenance sidecars for your records.
Do not commit your API key or any keystore to the repo.
