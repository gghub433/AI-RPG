# Armor, Helmets & Weapons

Equipment drives both stats and the character's on-body visuals. Implemented via
`ItemDefinition` (`category`, `slot`, `rarity`, `flatStatBonus`, `derivedStatBonus`,
`socketCount`) + `EquipmentManager`. Rarity ladder: Common, Uncommon, Rare, Epic,
Legendary, Mythical, Unique.

## Armor Sets (7)

| Set | Theme | Tier focus | Notes |
|---|---|---|---|
| **Iron Vanguard** | Heavy steel plate | Common–Rare | Early-game baseline, high physical defense, low mobility. |
| **Moonveil** | Silver elven warrior armor | Rare–Epic | Light/medium, agility + magic defense; Liana's aligned set. |
| **Ashen Knight** | Charred armor with ancient sigils | Epic | Curse-resistant; ties to Varinthe/ancient-civilization lore. |
| **Bloodfang** | Aggressive hunter armor | Epic–Legendary | Crit/lifesteal leaning; Kain/Kaela aligned. |
| **Dragon Sovereign** | Draconic plate | Legendary | High all-round stats; from a dragon-boss questline. |
| **Eclipse Sovereign** | Arden's legendary set | Unique | Arden-only; the Stage-2 hero evolution look (see `05_HeroEvolution.md`). |
| **Absolute Eclipse** | Final evolution | Unique | Arden-only; the Stage-3 endgame form. |

Set bonuses (wearing multiple pieces of a set) are a planned extension of
`EquipmentManager.RecomputeBonuses` — the recompute already sums all equipped pieces, so
a set-count pass can be layered on without reworking the pipeline.

## Helmet Types
Closed knight helms · magic masks · horned helms · ancient-king crowns · light scout
helms. Each occupies the `Head` slot; the Eclipse Sovereign Helm is the Unique-tier
full-face helm described in `05_HeroEvolution.md`.

## Weapon Types
One-handed swords · greatswords (two-handed) · daggers · spears · axes · bows · staves ·
spell tomes · shields · catalysts. Melee classes favor swords/daggers/greatswords/spears/
axes; casters favor staves/tomes/catalysts; no hard class lock, to allow experimentation.

### Notable weapons
- **Voidrend** (Unique greatsword) — Arden's signature; absorbs residual energy from
  defeated foes (links to `AbyssAbsorptionSystem`). Broad black blade, silver edge,
  ancient runes.
- Legendary/Unique weapons each carry a model, name, stats, origin story, usage
  requirements, and a special ability. Common/Uncommon weapons reuse shared base models
  re-skinned by material (per the "no faked finished art" rule — base models are
  placeholders until real art exists).

## Crystal Sockets
Weapons and armor may expose 0–3 crystal sockets (`ItemDefinition.socketCount`). Socketing
is handled by `CrystalSocketManager` and consumes a crystal from inventory; see
`Docs/Design/07_Crystals.md`.
