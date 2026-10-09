# Arc VII — War of Kingdoms

**Location:** Noble Terrace (Caldrin Vane, now unlocked) + the border region of Harrow March. **Playtime target:** 5 hours.

## Narrative Goals
Deliver the design brief's political-choice arc: the Thresh Dominion, the
**Clan of the Silver Moon**, and the **Demonic Houses** all move to claim a newly
exposed Varinthe energy source (discovered as fallout from Arc VI). Player choices
here determine which of three regional outcomes play out in Arc IX.

## Key Scenes & Quests

- **The Three Claims:** Castellan Dessa (Trade Union, recurring from Arc IV) brokers
  the player an audience with representatives of all three factions competing for
  the Varinthe energy site: Dominion Noble-Marshal **Edevane Thresh**, Silver Moon
  emissary **Isolde Rin**, and Demonic House envoy **Vash'tor Keel**. Each has a
  full personal-motive scene (not just a faction mouthpiece): Edevane genuinely
  believes Dominion control prevents a war; Isolde wants the site sealed, not used,
  for reasons tied to the Clan's own history with anomaly energy; Vash'tor wants it
  traded, not hoarded, because his House's standing depends on being a power-broker,
  not a power-holder.
- **Political quest chain (3 branches, player picks one to actively support — can
  also refuse all three and pursue a fourth "seal it, tell no one" ending only
  available if `arc6_curseweave_unlocked` and a high Ashlin Rell trust score):**
  each branch is a 3–4 quest chain with its own skirmish-scale battle set piece.
- **Companion stakes:** Serah Lind (if still in party) has a personal stake in the
  Dominion-support branch (it would fully clear Brisk's old case against her).
  Kael Thorne (recruitable as a companion starting this arc, if `arc3_kael_impressed`
  is true) has a stake in the Silver Moon branch (his family owes the Clan an old
  debt). This is where companion relationships start actively pulling against each
  other — the game's "morally complex decisions" requirement made mechanical, not
  just narrated.
- **Skirmish set piece:** whichever branch the player supports, a large-scale
  battle sequence (wave-based combat across a fortified pass, with companion-led
  sub-objectives) resolves the arc's main conflict.

## Flags Set at Arc End
- `arc7_faction_supported` (string: `dominion` / `silver_moon` / `demonic_house` / `sealed_site`)
- `arc7_kael_recruited` (bool)

## Rewards
~2500 XP, faction-specific Legendary weapon reward, Noble Terrace fully unlocked,
Kael Thorne as companion (if recruited), regional reputation track with all three
factions going forward.
