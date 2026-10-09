# World Bible

Authoritative region list for Rebirth: Eclipse System. Regions are implemented as
`RegionDefinition` assets (`Assets/Game/Scripts/World/`), unlocked through story flags via
`RegionProgressTracker`. Design rule: fewer, detailed, interactive spaces over vast empty
maps.

## The Eight Regions

1. **Ashenfall Village** — starter village. Farms, tavern, blacksmith (Garret Stone), first
   quests and the first enemies/mini-boss. The vertical-slice region.
2. **Eldoria** — large city: market, palace, guild halls. Hub for contracts, vendors,
   political threads.
3. **Blackspire City** — grim city of mercenaries and black-market traders. Vespera and
   Nora orbit here; gray-market artifacts via Fabian Mercer.
4. **Silvermoon Forest** — elven forest and ancient shrines. Liana's home; the
   crystal-harvest secret surfaces here.
5. **Frostveil Mountains** — snowbound peaks, mines, ice monsters. Source of ore and Frost
   Shard crystals; the Frost Queen boss.
6. **Crimson Wastes** — desert with the ruins of a dead civilization. Lucian Weir's
   lore-digging; crystal/System origin clues.
7. **Abyssal Citadel** — fortress tied to the System's origin. Mid/late-game; Abyssal
   Knight boss.
8. **Eclipse Throne** — final region of ancient power. Endgame; Malkar and the Source.

Each region carries its own visual theme, enemy roster, points of interest, side quests,
hidden areas, unique materials, mini-bosses, and story reveals, plus 2–4 side
dungeons/secret areas for repeatable endgame content (authored incrementally).

### Functional buildings
Homes, taverns, shops, temples, barracks, palaces, farms, mines, bridges, roads, secret
rooms, dungeons. Important buildings have functions: the player can buy housing, store
items, rest, meet NPCs, and trigger story scenes. Spaces are built detailed and
interactive rather than as large empty maps.

## Enemies & Bosses

**Standard enemies:** wolves, goblins, skeletons, bandits, cursed knights, mages, demons,
golems — each a distinct `EnemyArchetype` + `EnemyController` behavior, not one recolored
template.

**Named bosses** (each gets its own model, attack set, multiple phases, weaknesses,
animations, rewards, and story link via `BossController`):
Fallen Guardian · Crimson Dragon · Frost Queen · Abyssal Knight · The Betrayer ·
Malakar, World Devourer.

## Reconciliation with the earlier draft

An earlier worldbuilding pass (`Docs/Story/Arcs/*`, `Docs/Characters/00_`/`01_`,
`Docs/Design/02_Regions.md`) used a differently-named world (Veyth Hollow, Caldrin Vane,
companions like "Serah Lind," regions like "Hollow Reach"). That draft established the
coherent 12-arc narrative structure, systems architecture, and pacing — all of which
remain valid and name-agnostic (the C# systems reference nothing by these names).

This World Bible plus `Docs/Characters/02_Canonical_Cast.md` are the **canonical** names
going forward. The mapping is intentionally loose rather than a 1:1 rename, because the
two drafts describe overlapping-but-distinct casts; where they conflict, this file wins.
Fully rewriting the 12 arc documents to the canonical names is a follow-up task (not yet
done) — flagged in `Docs/Build/00_Status.md`. Until then, read the arc docs for
*structure and beats* and this file + the canonical cast for *names and world*.

| Earlier draft | Canonical | Notes |
|---|---|---|
| Veyth Hollow (starter town) | Ashenfall Village | same role: tutorial hub |
| Caldrin Vane (big city) | Eldoria | same role: guild/market hub |
| Hollow Reach / Varinthe deep sites | Abyssal Citadel / Eclipse Throne | System-origin + finale sites |
| Companion "Serah Lind" | (replaced) Serafina Walter / Mirael Nox split the role | investigator + System-scholar |
| Rival "Kael Thorne" | Darian Crowe | the rival-swordsman beat |
| Commander "Thessaly Kade" | Rhayne Vald | the betrayer-commander beat |
