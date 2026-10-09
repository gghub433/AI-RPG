# Arc XII — Absolute Abyss

**Location:** The Source Chamber (deepest Varinthe site, beneath the original Hollow Reach ruin). **Playtime target:** 5–7 hours including endings.

## Narrative Goals
Resolve the Eclipse System's origin, the "true owner" warning from the opening
cutscene, and deliver multiple distinct endings reflecting the player's accumulated
choices (Arc V, VII, X, XI flags all read here).

## Key Scenes & Quests

- **Descent:** the Source Chamber sits directly beneath the Arc I ruin — the game's
  final dungeon literally returns to where it started, now explorable in full for
  the first time (the collapsed Lower Gallery from Arc I is walkable again, with a
  full-circle environmental-storytelling pass: Reyes's remains, if the player looks,
  are still there, finally given a proper acknowledgment option).
- **The Source, revealed:** the Eclipse System's core intelligence is not a villain
  AI — it's the last coherent fragment of the Varinthe collective that built the
  continuity-failsafe System in the first place, still trying to complete its
  original task: find a stable anchor for the soul it was calibrated for before its
  civilization died. The cutscene-opening line from Arc I's death scene,
  **"ПРЕДУПРЕЖДЕНИЕ: ИСТИННЫЙ ВЛАДЕЛЕЦ НЕ УСТАНОВЛЕН,"** is finished here, word for
  word, by the Source itself: *"...and after this long searching, compatibility is
  no longer the question. Persistence is."* — the Source has decided Arden has
  earned the role through everything he survived, not through being a predestined
  match. This is the thematic payoff of the entire "power grows from real
  struggle, not fate" design pillar.
- **Director Vorne and the Cult's final move:** both factions converge on the Source
  Chamber simultaneously, each trying to seize control of the System for opposite
  reasons (Vorne to weaponize it for the Dominion, the Cult to force a transfer that
  serves their own ends) — final-dungeon gauntlet sections pit the player against
  both factions' forces, not just one final boss rush.
- **Final boss — "The Source, Unanchored" (multi-stage, 3 distinct phases plus a
  final dialogue-integrated phase):**
  - Phase 1: a physical guardian-form fight (tests everything: combo, dodge, skills).
  - Phase 2: a Curse/Void-saturated phase drawing on every resistance the player has
    built across the whole game (Abyss absorption resistances from Arcs II–XI
    directly reduce incoming damage here — a genuine payoff for having engaged with
    the mechanic rather than ignoring it).
  - Phase 3: a "memory gauntlet" — the player refights a condensed, lower-HP version
    of Caelus (Arc III) and the Rift Warden (Arc XI) back to back, framed as the
    Source testing whether Arden's growth was real.
  - **Final phase — "The Choice":** not a combat phase. A fully dialogue-driven
    decision point.

- **Endings (reflecting Arc V/VII/X/XI choices, at minimum four distinct epilogues):**
  1. **Become the Anchor** — Arden accepts the role permanently, merging with the
     System; epilogue shows the Dominion/Silver Moon/Demonic Houses balance
     stabilizing under his quiet, lasting influence (tone varies by `arc7_faction_supported`).
  2. **Sever the System** — Arden destroys the Source's continuity function,
     accepting he will age and die as Joren Calt for good, free of the System;
     epilogue focuses entirely on the family (Yulia, Pip) and companions, no
     Varinthe thread remains.
  3. **Hand it back** — if `arc10_kade_outcome == forgiven_allied`, a unique option:
     give the Source's remaining fragment to the Remnant Camp researchers to study
     safely rather than claiming or destroying it; epilogue centers Kade's redemption
     arc.
  4. **Walk Away Together** — available only if at least one romance was pursued to
     its final stage; the romanced companion gets a dedicated final scene
     regardless of which of endings 1–3 the player chose, making romance an epilogue
     modifier rather than a fifth exclusive branch.

## Flags Set at Arc End
- `arc12_source_confronted` (true) — major flag.
- `arc12_ending` (string: `anchor` / `severed` / `handed_back`)
- `arc12_romance_epilogue` (string: npc id or `none`)

## Rewards
Endgame-tier XP dump (level cap support), Unique-rarity "Source Fragment" relic
(cosmetic/NG+ hook), full epilogue slideshow reflecting the complete flag history —
no two playthroughs with meaningfully different Arc V/VII/X/XI choices should
produce an identical epilogue sequence.
