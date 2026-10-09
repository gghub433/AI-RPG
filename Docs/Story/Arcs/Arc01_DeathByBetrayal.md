# Arc I — Death by Betrayal

**Scene range:** Prologue (playable flashback). **Playtime target:** 25–35 min.
**Player character:** Arden Vale, Ashgrave Expedition surveyor-scout, in his original body.
**Unlocks at end:** Eclipse System activation, transition to Arc II.

## Narrative Goals

- Establish Arden's competence, his loyalty to his unit, and the specific flaw that
  gets him killed: he reports what he finds, on principle, even when told not to.
- Plant three visual/dialogue clues about Thessaly Kade's real motive that only pay
  off in Arc X (she is not hiding the anomaly for the Dominion — she is hiding it
  from someone in her own chain of command).
- Make the betrayal feel earned, not cartoonish: Kade gets a scene where her
  reasoning is coherent and the player can see how she talked herself into it.
- Teach core combat (light/heavy attack, dodge, block) and basic exploration in a
  context that doesn't feel like a tutorial — it's a survey team doing their job.

## Scene 1 — "The Lower Gallery" (gameplay: exploration + light combat)

**Location:** Hollow Reach Ruins, Lower Gallery B. **Party:** Arden, Captain Mireille
Oksana Dray (combat lead), Scout Tobin Reyes (comic relief, dies in Scene 3 — his
death is what removes any doubt that Kade means it).

- Player-controlled traversal: broken stair, one rope-descent set piece, two trash
  packs of **Hollow Wisps** (enemy type #1 — low HP, erratic movement, teaches dodge
  timing).
- Environmental storytelling: wall-mounted plaques in a script the UI subtitles as
  "pre-Collapse Dominion military," establishing the ruin predates the current
  Dominion by centuries. This is a Arc VI/VIII payoff seed.
- Dialogue beat (Dray, to Arden): *"You log everything, don't you. Even the stuff
  Kade tells you not to."* Arden's response here is a dialogue choice that sets a
  minor flag (`arc1_arden_honesty_stated`) referenced in two Arc X lines, not a
  mechanical branch — the story treats Arden as consistently principled regardless,
  but companions later quote this line back if it was voiced instead of shrugged off.

## Scene 2 — "The Anomaly" (gameplay: puzzle + discovery)

**Location:** Lower Gallery B, Sealed Vault.

- Light environmental puzzle: align three floor glyphs (simple rotate-and-match,
  teaches the interact prompt and camera-relative movement precision) to open the
  vault.
- Inside: the anomaly — a black, faceted object the size of a fist, suspended in a
  stasis field that predates every lock mechanism in the ruin. Scanning it (first use
  of the "scan" prompt, foreshadowing Eclipse System's enemy-scan feature) returns
  only: **"CLASSIFICATION: PENDING. COMPATIBILITY: PENDING."**
- Dray orders it bagged for Kade. Arden insists it also goes in the written survey
  log, per protocol — Dray doesn't stop him, but visibly hesitates. (Plant: Dray
  knew, or suspected, what Kade would do. Pays off in Arc X Dray reappearance.)

## Scene 3 — "The Order" (cutscene, no player input)

**Location:** Surface camp, command tent.

Kade reads the survey log entry naming the anomaly. Full dialogue:

> **KADE:** You logged it.
> **ARDEN:** It's protocol.
> **KADE:** Protocol's for things we understand. We don't know what this reports to,
> back home, or what it's worth to whoever reads that line before I get a say in it.
> **ARDEN:** That's not your call to make alone.
> **KADE:** It's the only call anyone down here is in a position to make. (*beat*)
> I'm sorry, Vale. You're a good scout. This isn't about you.

Kade gives the order to Reyes and two other soldiers to seal the lower gallery —
with Arden still inside, having gone back for a dropped logbook page (player-directed
micro-objective, 30 seconds earlier, so his presence there is the player's own
action, not a cutscene contrivance). Reyes hesitates and is the one who triggers the
charges late and badly — the ceiling partially collapses instead of sealing cleanly,
which is *why* Arden survives long enough to reach the anomaly again instead of
dying instantly. Reyes dies in the same collapse.

## Scene 4 — "What Answers in the Dark" (gameplay: slow walk + first System UI)

**Location:** Lower Gallery B, collapsed section.

- Arden, critically wounded (health locked at 1, no combat, movement slowed),
  crawls back toward the vault. This is deliberately uncomfortable pacing — the one
  moment in the whole game the player is NOT meant to feel powerful.
- He reaches the anomaly. It is already active, already selecting him — the
  stasis field clues from Scene 2 are explained retroactively: the anomaly wakes for
  dying minds, not living ones.
- First System panel, exactly as specified in the design brief:

```
[ОБНАРУЖЕНА АНОМАЛИЯ ДУШИ]
[ПРОВЕРКА СОВМЕСТИМОСТИ]
[ПРОТОКОЛ ПЕРЕРОЖДЕНИЯ АКТИВИРОВАН]
[ДОСТУП К СИСТЕМЕ БЕЗДНЫ ОТКРЫТ]
[ПРЕДУПРЕЖДЕНИЕ: ИСТИННЫЙ ВЛАДЕЛЕЦ НЕ УСТАНОВЛЕН]
```

- Screen fades on Arden's hand closing around the anomaly. Cut to black mid-sentence
  of a line that will be finished, word for word, at the start of Arc XII.

## Flags Set at Arc End

- `arc1_betrayal_witnessed` (true) — major flag, raises Eclipse Sync Rate.
- `arc1_dray_hesitated` (true) — gates a Dray-specific dialogue option in Arc X.
- `arc1_reyes_died` (true) — referenced in a Arc IV side-quest (Reyes's sister looking
  for him in the city of Caldrin Vane).
- `arc1_arden_honesty_stated` (optional, from the Scene 1 dialogue choice).

## Rewards

None — Arc I is pre-System, no XP/items exist yet. The "reward" is narrative and the
unlocked Eclipse System itself at the top of Arc II.
