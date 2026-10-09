# Arc V — Betrayal Within the Party

**Location:** Caldrin Vane (continued) + Greywatch Pass. **Playtime target:** 3 hours.

## Narrative Goals
Deliver the design brief's required "close ally hides the truth" arc, with a
player-decided outcome — this is the first arc whose resolution meaningfully forks
later content (a companion's availability, not just dialogue flavor).

## Key Scenes & Quests

- **Serah's secret:** Serah Lind has been quietly feeding information about Arden's
  growing capabilities to her former Guild handler, **Inspector Calloway Brisk** —
  not out of malice, but because Brisk is dangling reinstatement to her real Rank B
  in exchange for it, and she genuinely believes she's protecting Arden from a worse
  outcome (Brisk otherwise flags him as an unregistered anomaly, which under Dominion
  law means detainment). This is deliberately a sympathetic betrayal, not a cartoon
  one — matching the design brief's "not every ally becomes a traitor, and when one
  does, it's foreshadowed" rule (Arc IV already planted that Serah's demotion is
  connected to artifact cover-ups; this arc reveals she never stopped being
  entangled in it).
- **Discovery ("The Second Report"):** the player finds a drafted report in Serah's
  effects during a shared camp scene at Greywatch Pass (optional exploration beat —
  missable if the player doesn't search camp, which is intentional: thorough players
  are rewarded with earlier knowledge and an extra dialogue option).
- **Confrontation (branching):**
  1. **Confront immediately** — Serah admits it, the two have a full voiced-dialogue
     argument; player can forgive (she stays, loyalty hit but recoverable), demand
     she cut contact with Brisk on-screen (she does, small boss-adjacent side
     encounter against Brisk's enforcers who come to collect), or sever the
     companionship (she leaves the party for the rest of Arc V–VII, returning
     changed in Arc VIII regardless, per the "don't let every thread stay severed
     forever" story rule).
  2. **Let it ride, investigate quietly** — leads to a side-dungeon ("Brisk's Ledger
     House") where the player gets hard evidence of the artifact-smuggling ring Brisk
     actually runs, reframing Serah as a double-agent who was using the leak to feed
     Arden real intel about Brisk in return. This path unlocks a unique scene where
     Serah reveals she'd already decided to come clean.
- **Mini-boss — "Brisk's Enforcers":** a 3-enemy squad fight (one shield-caster, two
  duelists) that plays very differently depending on branch (ambush vs. open
  confrontation changes starting positions and whether the player gets a surprise
  attack).

## Flags Set at Arc End
- `arc5_serah_outcome` (string: `forgiven` / `severed_contact` / `left_party` / `double_agent_revealed`)
- `arc5_brisk_exposed` (bool, true only on the investigate branch)

## Rewards
~900 XP, Epic-tier companion-exclusive weapon for Serah if she stays, access to
Brisk's Ledger House loot (Epic armor piece) if investigated.
