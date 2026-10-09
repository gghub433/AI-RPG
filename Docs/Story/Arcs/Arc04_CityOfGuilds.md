# Arc IV — City of Guilds

**Location:** Caldrin Vane, the Dominion's largest free city. **Playtime target:** 3–4 hours.

## Narrative Goals
Open the game's first true hub city and guild-contract economy; introduce the
Imperial Guild, the Trade Union, and Arden's first long-term companion.

## Key Scenes & Quests

- **Arrival ("Gates of Vane"):** first fast-travel network node; fully explorable
  district layout (Guild Quarter, Market Span, Lower Docks, Noble Terrace — Noble
  Terrace locked until Arc VII). Arden registers with the **Imperial Guild** under
  Joren's name, taking a provisional **Rank F contractor** tag — the game keeps
  reminding the player that the world's official measure of Arden is still wrong,
  even as his real power grows.
- **Companion recruitment — "Serah Lind":** a disgraced ex-Guild investigator
  (Rank C, formerly B before a cover-up cost her the rank) hired on a bounty board
  job that turns out to be Arden's. She becomes the first full combat companion.
  Her personal quest thread (introduced here, developed in Arc V) concerns the
  cover-up that demoted her — which turns out to connect to the Imperial Guild's
  quiet trade in "anomalous" artifacts, a direct thematic echo of Arc I.
- **Reyes's sister ("The Unclaimed Dead"):** side quest using the `arc1_reyes_died`
  flag — Mira Reyes, working the Lower Docks, is searching for her brother's unit.
  The player can tell her the truth, lie to spare her, or deflect. Each choice sets
  `arc4_mira_told_truth` and changes one line of dialogue in Arc IX when Mira
  reappears as a refugee-camp organizer.
- **Guild contracts (repeatable infrastructure):** the Imperial Guild's bounty board
  is introduced here as the game's first repeatable-quest hub (bounty hunts, escort
  jobs, material-gathering contracts) — optional, never gating main story progress,
  per the design brief's "no artificial grind gating" rule.
- **Mini-dungeon — "The Flooded Archive":** a secondary dungeon under the Lower
  Docks, required for Serah's recruitment quest. Introduces the game's first
  swimming/traversal variant enemy, the **Brine Lurker** pack, and a light puzzle
  (drain three floodgates in the correct order using a wheel-and-valve mechanism).
- **Faction intro — Trade Union:** a merchant-prince NPC, **Castellan Orve Dessa**,
  offers the player's first faction-reputation track, tied to future crafting
  material access rather than story gating.

## Flags Set at Arc End
- `arc4_serah_recruited` (true)
- `arc4_mira_told_truth` (bool)
- `arc4_guild_rank` (int, starts at 1)

## Rewards
~1200 XP across quests/dungeon, Serah Lind as permanent companion, Rare-tier gear
from the Flooded Archive boss chest, Imperial Guild contractor perks (vendor
discount), fast travel hub unlocked.
