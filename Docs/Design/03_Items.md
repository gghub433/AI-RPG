# Items & Equipment

Rarity tiers (ascending): Common, Uncommon, Rare, Epic, Legendary, Mythical, Unique
(`RebirthEclipse.Inventory.ItemRarity`). Every item carries real stat/effect data
(`ItemDefinition`) — none are cosmetic-only unless explicitly a cosmetic slot item
(cosmetic slots are a planned post-launch addition, out of scope for the first
chapter).

## Categories
- **Weapons:** swords, daggers, two-handed weapons, staves, mana catalysts — one
  weapon family per class archetype (melee classes favor swords/daggers/two-handed;
  caster classes favor staves/catalysts), but no hard class lock, so experimentation
  is allowed.
- **Armor:** head/chest/legs, three armor "weight" families (light/medium/heavy)
  affecting moveSpeed vs. defense trade-offs via `DerivedStats`.
- **Accessories:** rings (x2 slots), amulets — the primary vector for build-defining
  unique effects (e.g., "+1 max Abyss absorption per rest" on a Rare+ ring).
- **Consumables:** health/mana/stamina draughts (3 tiers each), buff tonics,
  "Memory Thread" (free skill respec).
- **Materials & crafting components:** region-exclusive materials (see
  `02_Regions.md`) feed the crafting system; higher-rarity gear always requires at
  least one region-exclusive material, incentivizing exploration over pure grinding.

## Rarity Curve Guidelines
| Rarity | Approx. drop source | Stat budget (relative) |
|---|---|---|
| Common | trash mobs, vendors | 1.0x |
| Uncommon | mini-bosses, quest rewards | 1.3x |
| Rare | dungeon chests, side-quest rewards | 1.7x |
| Epic | bosses, faction reputation rewards | 2.2x |
| Legendary | arc-ending bosses, major quest rewards | 2.8x |
| Mythical | optional hard content, hidden quests | 3.5x |
| Unique | one-of-a-kind, story-tied (named effect, not just bigger numbers) | varies |

## Random Affixes
Gear flagged `hasRandomAffixes` rolls up to 3 bonus affixes at drop time (see
`ItemDefinition.maxRandomAffixes`); affix pools are rarity-gated (Common items never
roll affixes; Legendary+ always roll the maximum). Rolled values are stored on the
specific `ItemStack.rolledBonus`/`uniqueInstanceId`, never on the shared
`ItemDefinition`, which is what keeps two drops of "the same" Legendary sword
meaningfully different without needing two separate assets.

## Crafting
Recipes are `ItemDefinition` assets with `isCraftable = true` and a
`requiredMaterialIds`/`requiredMaterialCounts` pair; `InventorySystem.TryCraft`
validates and consumes materials atomically — a craft either fully succeeds or
changes nothing, preventing material loss on a failed craft.
