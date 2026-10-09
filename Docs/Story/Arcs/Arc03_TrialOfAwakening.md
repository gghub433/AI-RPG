# Arc III — Trial of Awakening

**Scene range:** Aldenwatch Spire dungeon (first full dungeon, first mini-boss, first boss).
**Playtime target:** 1.5–2.5 hours. **Unlocks at end:** First class assignment, Arc IV.

## Narrative Goals

- Deliver the first real dungeon structure (3 floors, escalating enemy variety,
  one mini-boss, one full boss) as the vertical-slice combat showcase.
- Introduce **Kael Thorne** — a noble-born, naturally Rank A Aptitude prodigy sitting
  the same Trial out of boredom/expectation — as Arden's first rival. Kael is not a
  villain; he is a foil who starts dismissive and ends the arc genuinely unsettled by
  what Joren turns out to be.
- First class assignment — the moment the System stops reading "Unknown."
- Teach: boss telegraphs, status effects (the boss inflicts Curse), and the first
  meaningful choice between finishing the dungeon stealthily or thoroughly (affects
  loot, not story).

## Floor 1 — "The Outer Vault" (gameplay: mixed trash combat, 2 enemy types)

- Enemy types: **Spire Wisps** (ranged mana bolts, teaches target-lock against
  ranged threats) and **Rustback Constructs** (slow, heavy melee, teaches
  heavy-attack trade timing and stamina management).
- Kael Thorne appears, cuts through a construct in two hits in front of the player,
  needles Joren ("Null boy. Didn't expect you to still be breathing.") and moves
  ahead, setting up a soft race-the-rival structure for the floor.
- Side-path: a sealed side-vault requiring a lever puzzle (two levers, must be
  thrown within 3 seconds of each other — solo-friendly timing puzzle) containing
  the Trial's one piece of Uncommon gear, **Scoutwalker Boots** (+move speed).

## Floor 2 — "The Grieving Hall" (gameplay: mini-boss)

**Mini-boss: The Grieving Warden** — an animated suit of Dominion-military armor
(visual/lore callback to Arc I's pre-Collapse plaques). Single-phase but a genuine
skill check: telegraphed overhead slam (dodge-timing check), a sweeping horizontal
attack (requires repositioning, not just blocking), and a Curse-infused grab attack
that must be escaped by button-mash or it applies a stacking debuff.

- On defeat: first scannable "important" enemy — scanning the Warden before or after
  the fight reveals a locked memory fragment (`fragment_warden_ashgrave`), which, if
  the player chooses to absorb the Warden's residual energy, unlocks a short memory
  vision: a glimpse of the *same pre-Collapse military* seen in Arc I's ruin,
  confirming the two sites are connected centuries apart. This is optional and
  easy to miss — rewarding thorough players with real lore, not padding.

## Floor 3 — "The Awakening Chamber" (gameplay: full boss fight)

**Boss: Caelus, the Unclaimed Flame** — a Trial-appointed guardian construct whose
entire design intent is "the Trial calibrates its guardian to the challenger," which
is the diegetic excuse for Caelus being a genuine multi-phase fight instead of a
reused mini-boss pattern.

**Phase 1 (100%–65% HP):** Standard melee/ranged mix, player learns the arena's
destructible pillars (can be used to block Caelus's ranged volley).
**Phase 2 (65%–30% HP):** Telegraph: Caelus kneels and the arena's torches gutter
blue. Caelus gains a Curse-infused attack set and periodically marks the player with
a "Weakness Exposed" status (visibly tagged in the Eclipse HUD) — the fight is
teaching the player to read status icons, not just health bars.
**Phase 3 (30%–0%, scripted at 30%):** Short cutscene-interrupt: Caelus roars, the
chamber's ceiling murals light up showing the same anomaly glyphs from Arc I's
vault. Caelus's final phase adds a screen-telegraphed ultimate ("Flame Judgment") —
a clearly animated 2-second wind-up the player must dodge-roll through or block with
heavy stamina cost; getting hit deals near-lethal damage but is not an instant kill,
keeping the fight fair for a first-time boss player.

On defeat, Caelus's core is absorbable and story-flagged as a **required** absorption
(the only one in the game that isn't optional) — it triggers the class assignment
scene directly.

## Scene — "Classification" (cutscene + first real class choice)

The Eclipse System, now sync'd enough from the Caelus absorption, resolves Arden's
class for the first time. Mechanically: the player is shown their stat distribution
and combat behavior from the Trial (tracked flags: hits blocked vs. dodged, melee vs.
skill usage, absorption used) and offered a **recommended** class with two
alternates, rather than a hard-locked result — respecting player agency while still
making the choice feel earned from how they actually played.

```
[КЛАССИФИКАЦИЯ ЗАВЕРШЕНА]
Рекомендованный класс: Теневой клинок
Альтернативы: Рыцарь Бездны · Пожиратель энергии
```

Kael Thorne, having also finished the Trial (off-screen, implied), finds Arden
outside the Spire and has the arc's final dialogue — openly unsettled that a
"Rank F null" out-fought a guardian construct built to scale to real Aptitude. He
doesn't become friendly yet; he becomes *curious*, which is the seed for his Arc V
role.

## Flags Set at Arc End

- `arc3_first_awakening` (true) — major flag, raises Eclipse Sync Rate.
- `arc3_class_chosen` (string: class id)
- `arc3_fragment_warden_found` (bool, optional)
- `arc3_kael_impressed` (true)

## Rewards

~600 XP across the dungeon, Uncommon Scoutwalker Boots, Rare starter class-specific
weapon (varies by chosen class), first skill tree points to spend, unlocked fast
travel point "Aldenwatch Spire."
