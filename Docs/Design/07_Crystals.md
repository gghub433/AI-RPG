# Magic Crystals & Energy

Implemented by `Assets/Game/Scripts/Crystals/` (`CrystalDefinition`, `CrystalDatabase`,
`CrystalSocketManager`). Crystals are gear modifiers and crafting inputs; socketing one
applies its derived-stat bonus and, on weapons, tags outgoing hits with its school's
damage type.

## The Seven Schools

| Crystal | School | Role | Weapon damage type |
|---|---|---|---|
| **Flame Core** | FlameCore | Fire, burst/burn damage | Magical |
| **Frost Shard** | FrostShard | Cold, slow/control | Magical |
| **Storm Crystal** | StormCrystal | Lightning, speed | Magical |
| **Earthheart** | Earthheart | Defense, stability | Physical |
| **Spirit Gem** | SpiritGem | Healing, support | Magical |
| **Void Crystal** | VoidCrystal | Energy absorption, curses | Void |
| **Eclipse Core** | EclipseCore | Arden's rarest power source | Void |

## Quality Tiers
Flawed (×0.6) · Standard (×1.0) · Radiant (×1.5) · Perfect (×2.2). Quality multiplies the
crystal's raw socket bonus (`CrystalDefinition.QualityMultiplier`), so one asset family
scales cleanly without a separate asset per tier.

## Uses
- **Socketing** into weapons/armor (`CrystalSocketManager.TrySocket`) — limited by the
  item's `socketCount`; socketing consumes the crystal, unsocketing returns it. No crystal
  is created or destroyed by socketing, keeping them on the same anti-duplication footing
  as other items (covered by `CrystalSocketTests`).
- **Crafting** inputs for weapons/armor (`CraftingRecipe.requiredCrystalIds`).
- **Activating ancient mechanisms** and **brewing potions** (world/quest hooks, data-driven).

## Rules & Limits
- **Compatibility gate:** a crystal flagged `restrictedToAlignedGear` only sockets into
  gear that accepts its school (`CrystalSocketManager.GearAcceptsSchool`). Eclipse Core is
  the strictest — it only sockets into Unique/Mythical gear, so it can't be dropped into
  anything.
- **Upgrade risk:** improving a crystal's quality is a crafting recipe (`CraftKind.Upgrade`)
  with a success chance that falls off at higher levels — a failed attempt consumes
  materials (the gamble) but, with `safeFailure`, does not reduce the crystal.
- **Eclipse Core** is tied to Arden's own progression (hero stages / story), not sold or
  farmed freely.

## VFX per school
Each school defines particles, attack trails, flashes, rune circles, and hit effects
(addressable/prefab keys, to be authored in-editor). Intensity scales with the Android
graphics preset (`QualitySettingsManager`), so heavy effects don't run full-blast on
mid-tier devices.
