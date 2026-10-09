# Arden — Hero Evolution Stages

Implemented by `Assets/Game/Scripts/Character/HeroStageController.cs` (data = `HeroStage[]`).
Each stage swaps Arden's armor/weapon visual set and applies a flat power multiplier, and
advancement requires **both** a story flag and an Eclipse Sync Rate threshold — never
grind alone. Stages are permanent once achieved (verified by `HeroStageTests`).

The character silhouette stays recognizable across all four stages (black/silver palette,
violet accents); each stage adds, never fully replaces, the identity.

## Stage 0 — Reborn
- **Gate:** none (starting stage, from Arc II).
- **Look:** scavenged commoner gear over the "weak" body he wakes in; no aura.
- **Weapon:** a plain starting blade.
- **Power multiplier:** 1.0.

## Stage 1 — Abyss Awakening
- **Gate:** story flag `arc3_first_awakening` + Sync Rate ≥ 20%.
- **Look:** first Abyss-touched armor pieces — dark plating with faint violet veining,
  asymmetric left pauldron begins to form.
- **Weapon:** an Abyss-infused blade (early Voidrend precursor).
- **Power multiplier:** ~1.3.
- **Aura VFX:** low violet shimmer on heavy attacks.

## Stage 2 — Eclipse Sovereign
- **Gate:** story flag `arc8_system_truth_revealed` + Sync Rate ≥ 50%.
- **Look:** the signature set — **Eclipse Sovereign**: black obsidian metal, silver edges,
  asymmetric pauldrons, layered breastplate with a magic crystal set in the chest,
  armored gauntlets, greaves, and a long black cloak with a violet interior. The
  **Eclipse Sovereign Helm** fully encloses the face: narrow visor, sharp silver
  detailing, crown-like crests.
- **Weapon:** **Voidrend** — legendary greatsword, broad black blade, silver edge, ancient
  runes, unusual guard; absorbs residual energy from defeated foes (ties to the Abyss
  absorption system).
- **Power multiplier:** ~1.8.
- **Aura VFX:** steady eclipse-corona aura; weapon trail on skills.

## Stage 3 — Absolute Eclipse
- **Gate:** story flag `arc12_source_confronted` + Sync Rate ≥ 85% (endgame).
- **Look:** the final evolution of the Sovereign set — the crystal and runework spread
  across the whole silhouette; the cloak becomes a field of eclipse-light.
- **Weapon:** Voidrend in its awakened form, matching the Absolute Eclipse set.
- **Power multiplier:** ~2.4 (endgame-tuned).
- **Aura VFX:** full-body eclipse field; unique ultimate tied to the Arc XII ending chosen.

## Authoring Notes

- The visual swap (armor/weapon/aura) is driven by a presentation-layer listener on
  `GameEvents.OnHeroStageChanged`; `HeroStageController` owns only the gameplay state so
  the two concerns stay separable (and the controller is testable without any art).
- `armorSetId` / `weaponId` / `auraVfxId` on each `HeroStage` are addressable/prefab keys
  to be wired once the real models exist. No placeholder primitives are shipped in their
  place.
- Power multipliers above are design targets for balancing against the boss roster in
  `Docs/Design/09_...` (TBD) and will be tuned once combat is playable in-editor.
