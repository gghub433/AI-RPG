# Arc IX — The Fall of the Capital

**Location:** Caldrin Vane (under siege). **Playtime target:** 4–5 hours.

## Narrative Goals
Deliver the design brief's required large-scale world-changing event: a faction
war (seeded in Arc VII) escalates into open siege of Caldrin Vane, permanently
altering the hub city, closing some trade routes, and killing at least one named
secondary character whose death is fully traceable to earlier player decisions.

## Key Scenes & Quests

- **Trigger:** whichever faction was NOT supported in Arc VII (or, on the
  `sealed_site` branch, a fourth opportunist — the **Cult of the Hollow Depths**,
  first named here, reserved as Arc XI/XII's deeper antagonist group) moves on
  Caldrin Vane to seize the Varinthe energy site by force, having run out of
  patience with diplomacy.
- **The Siege (multi-stage event, not one battle):** District-by-district combat
  across the Market Span, Lower Docks, and Guild Quarter, each with its own
  mid-boss-tier siege commander and its own rescue-or-abandon civilian objective
  (time-pressured but generous — failing one doesn't fail the arc, it changes which
  NPCs appear in Arc X/XI as refugees vs. survivors).
- **Consequence — Mira Reyes:** if `arc4_mira_told_truth` is false (the player lied
  to her about her brother), Mira's refugee-camp organizing goes worse under siege
  pressure and she can die defending a civilian convoy the player arrives at just
  too late — a direct, traceable callback: lying to spare her feelings in Arc IV
  left her without closure, which the story frames as *why* she takes an unnecessary
  risk here (she's still looking for an answer the player could have given her
  earlier). Telling the truth in Arc IV makes this same beat resolvable instead.
- **Dray returns:** Captain Dray (Arc I) reappears, now a city-watch commander in
  Caldrin Vane, leading one district's defense. First reunion scene with Arden —
  she does not recognize Joren/Arden's face, but his combat style, if the player
  favors blocking/parrying (callback to her Arc I combat-lead role), earns a
  pointed "you fight like someone I used to know" line.
- **Boss — "The Siege-Breaker":** the opposing faction's champion, a genuine
  full-scale multi-phase boss fought across a collapsing district set piece
  (environmental hazards change the arena mid-fight, not just the boss's attacks).

## Flags Set at Arc End
- `arc9_capital_fallen_districts` (list of district ids damaged/lost)
- `arc9_mira_outcome` (string: `survived` / `died`, dependent on Arc IV flag)
- `arc9_dray_reunion` (true)

## Rewards
~3000 XP, Legendary siege-commander loot, permanent world-state change (certain
Market Span vendors relocate/close for the rest of the game, reflected in NPC
schedules), access to Dray as a late-recruitable companion in Arc X.
