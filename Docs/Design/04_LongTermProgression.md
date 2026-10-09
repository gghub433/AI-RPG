# Long-Term Progression Plan (~10-Month Target Content Arc)

This is a target scope/cadence for post-launch content delivery, not a requirement
that players wait ten months for an ending — the 12 story arcs described in
`Docs/Story/Arcs/` are the complete main campaign, playable start to finish in the
first release. This document describes how that release is staged and what grows
around it afterward.

## Stage Breakdown

1. **Rebirth & Tutorial** (Arcs I–II) — ships in the first playable vertical slice.
2. **First Guilds & Classes** (Arc III–IV) — first class system + guild hub.
3. **Companions & Relationships** (Arc IV–V onward) — companion recruitment,
   romance tracks, relationship consequences.
4. **New Regions & Dungeons** (Arc VI) — first major post-hub region expansion.
5. **Class Evolutions** (ongoing, gated by Arc flags per `01_Classes.md`).
6. **War of Factions** (Arc VII) — political branching system.
7. **High-Level Bosses** (Arc VII–IX) — escalating boss design complexity.
8. **Revelation of the Past** (Arc VIII, X) — major lore payoffs.
9. **Final Trials** (Arc XI) — hardest standard content before the finale.
10. **Endgame & Alternate Routes** (Arc XII + post-launch) — multiple endings,
    then repeatable endgame systems below.

## Repeatable / Optional Systems (never gate main-campaign progress)

- **Daily/weekly contracts** via the Imperial Guild bounty board (Arc IV
  infrastructure) — entirely optional, reward currency/materials, no energy system,
  no paywall.
- **Repeatable dungeons** — each major region's 2–4 side dungeons (per
  `02_Regions.md`) support scaled repeat clears for farming materials/affixes.
- **Trials/challenge modes** — fixed-seed combat challenges (e.g., "no-hit Caelus,"
  "Vharok under 3 minutes") for leaderboard-style mastery goals, added
  incrementally post-launch rather than shipped in full day one.
- **Boss hunting** — optional superboss-tier encounters seeded in each late-game
  region, explicitly separate from story-required bosses.
- **Companion development** — post-Arc-XII epilogue-compatible companion vignette
  quests, expanding each companion's resolved arc with smaller optional content.
- **Rare items & secret classes** — the Absolute Abyss hidden class
  (`01_Classes.md`) and its equivalent-tier gear remain long-term chase goals.
- **Achievements** — tracked via `EclipseSystemModel`'s secrets log and a planned
  dedicated `AchievementDatabase` (not yet implemented; flagged as a known gap in
  the project status report).

## Explicit Anti-Patterns (per design brief, enforced here)

- No stamina/energy system limiting how much of the campaign can be played per day.
- No required microtransactions gating main-campaign content.
- No padding main-campaign length with repeated empty filler quests — if content is
  short at any stage, the plan is to ship less but polished rather than pad with
  busywork, and extend via the repeatable systems above instead.
