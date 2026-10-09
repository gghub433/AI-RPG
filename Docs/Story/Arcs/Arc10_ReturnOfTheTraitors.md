# Arc X — Return of the Traitors

**Location:** The Ashgrave Remnant Camp (survivors of Arc I's expedition, relocated) + Hollow Reach Ruins (second return). **Playtime target:** 4 hours.

## Narrative Goals
Recontextualize Arc I's betrayal with new facts, without erasing Kade's
culpability — the design brief explicitly calls for "revealing facts that change
perception of the original betrayal," not for retroactively excusing it.

## Key Scenes & Quests

- **The Remnant Camp:** surviving Ashgrave Expedition members, including
  **Thessaly Kade** herself, have been living in hiding — not from guilt alone, but
  because Kade's superiors (the actual answer to "who was she really hiding the
  anomaly from") turned on the expedition's chain of command once the anomaly
  vanished from the sealed gallery (i.e., once Arden took it with him into the
  Eclipse transfer). Kade has spent years believing she caused a catastrophe by
  losing track of a Class-Zero anomaly, with no idea Arden survived at all.
- **Confrontation (the arc's emotional core):** Arden, recognized this time — not by
  face, but because he can only know details about that night that the real Arden
  Vale would know — reveals himself to Kade. Her reaction is not relief; it's horror
  that the order she gave *worked exactly as she designed it not to*: she sealed the
  gallery to protect Arden from her superiors' reach, planning to dig him out once
  they were gone, and Reyes's premature detonation (Arc I) ruined that plan without
  her ever learning it. **This is the recontextualization**: Kade's betrayal was
  real and still got a man killed, but her intent was containment-to-protect, not
  disposal — explaining Dray's Arc I hesitation (`arc1_dray_hesitated`: Dray partly
  knew the plan) without excusing the outcome.
- **Player choice (major, no "correct" answer):** forgive Kade and recruit the
  Remnant camp as an ally faction for the endgame; reject her and leave the camp to
  its fate against the superiors who are still hunting it; or expose the truth
  publicly through Dray's city-watch position (available if `arc9_dray_reunion` is
  true), triggering a political fallout sub-quest instead.
- **The Superiors, named:** this arc names **Director Ansel Vorne** of the Dominion's
  black-budget "Anomaly Containment Directorate" as the figure Kade was hiding the
  find from — planting Vorne directly as Arc XI/XII's human-institutional antagonist,
  distinct from the Cult of the Hollow Depths' more cosmic threat.
- **Mini-boss — "Vorne's Reclaimers":** a Directorate strike team sent to finally
  silence the Remnant camp, arriving mid-arc regardless of player choice — this
  fight happens no matter what, keeping the arc from resolving too quietly.

## Flags Set at Arc End
- `arc10_kade_outcome` (string: `forgiven_allied` / `rejected` / `exposed_publicly`)
- `arc10_vorne_named` (true)

## Rewards
~2800 XP, Remnant Camp as an unlockable late-game hub (if allied), Epic
Directorate-tech gear from the Reclaimers, Kade as a non-combat strategic ally
(war-room buffs in Arc XI/XII) if allied.
