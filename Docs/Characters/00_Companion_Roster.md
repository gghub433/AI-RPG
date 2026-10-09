# Companion & Major NPC Roster

20+ significant NPCs, each with the fields required by the design brief: name, age,
appearance, voice style, personality, history, motivation, personal quest, secrets,
relationship to Arden, possible changes, story consequences. Grouped by first
appearance arc. Full personal-quest scripts live alongside each companion's
dedicated quest chain documents (to be expanded per-arc as implementation proceeds);
this document is the canonical reference sheet.

---

## Playable Companions

### Serah Lind
- **Age:** 27. **Appearance:** close-cropped auburn hair, a Guild-issue coat worn
  past its official retirement, a burn scar along her left forearm she never covers.
- **Voice style:** dry, economical, drops formal titles the instant she trusts someone.
- **Personality:** investigator's patience wrapped around a temper she's spent years
  learning to time correctly.
- **History:** former Rank B Imperial Guild Investigator, demoted to Rank C and
  exiled to bounty work after refusing to bury a report on artifact smuggling within
  her own Guild chain — the same smuggling ring that later entangles her with
  Inspector Brisk.
- **Motivation:** reinstatement was never really the point; proving the cover-up
  existed is.
- **Personal quest:** Arc IV (recruitment) → Arc V (the Brisk entanglement,
  branching) → Arc VIII (reaction to Eclipse System truth).
- **Secrets:** fed information to Brisk under duress/incentive (Arc V reveal).
- **Relationship to Arden:** professional respect curdling into real loyalty; a
  romance track unlocks if affinity crosses threshold after Arc V resolves non-negatively.
- **Possible changes:** can leave the party permanently (Arc V `left_party` branch,
  partially reversible in Arc VIII), can achieve full reinstatement (Arc X, if
  `arc5_brisk_exposed` true and player assists a short follow-up quest).
- **Consequences:** her loyalty state directly affects available dialogue/options in
  Arcs VII, VIII, X, XII.

### Kael Thorne
- **Age:** 20. **Appearance:** Silver Moon noble fashion, deliberately unscuffed
  armor he treats as a costume more than gear, until the story makes him earn it.
- **Voice style:** clipped, performatively bored, cracks into genuine curiosity when
  surprised — which Arden causes often.
- **History:** naturally Rank A Aptitude, heir to a minor Silver Moon-aligned house
  with an old debt to the Clan proper.
- **Motivation:** starts the game bored by his own talent; ends it needing to prove
  his choices matter more than his birth rank.
- **Personal quest:** Arc III (rival intro) → Arc VII (Silver Moon debt, recruitment)
  → Arc XI (life-or-death payoff if debt thread unresolved).
- **Secrets:** his family's debt to the Clan was incurred covering up a Thorne
  ancestor's failed anomaly experiment — another quiet Varinthe echo.
- **Relationship to Arden:** rivalry → grudging respect → genuine friendship;
  romanceable only on specific player dialogue path (not gender-restricted).
- **Possible changes:** can die in Arc XI if his debt thread is never addressed.
- **Consequences:** his survival/death changes two companion-banter scenes in Arc XII
  and removes/adds his unique weapon from the final-arc loot pool.

### Thessaly Kade
- **Age:** 44. **Appearance:** grey-streaked dark hair kept military-short, a
  Directorate-issue coat she's modified to remove every rank insignia.
- **History:** Arc I's commander and the direct architect of Arden's death — see
  Arc I and Arc X documents for her full arc.
- **Motivation:** believed containment would protect Arden from Director Vorne's
  Directorate; has spent years grieving a death she caused by accident, not design.
- **Personal quest:** Arc I (antagonist) → Arc X (recontextualization, recruitment
  as strategic ally) → Arc XII (redemption ending branch).
- **Relationship to Arden:** the game's central betrayal-and-reconciliation arc; not
  romanceable (deliberate — the story treats this relationship as something other
  than a romance option).
- **Possible changes:** allied strategic NPC, rejected antagonist-adjacent figure, or
  publicly exposed political casualty, per Arc X branch.

### Captain Mireille Dray
- **Age:** 38. **Appearance:** broad-shouldered, a long scar through one eyebrow
  from the Hollow Reach collapse itself.
- **History:** Arc I combat lead, survived, now a Caldrin Vane city-watch commander.
- **Motivation:** has spent years uneasy about Kade's order without ever having
  proof something was wrong.
- **Personal quest:** Arc IX (reunion) → Arc X (late-recruitable companion, combat
  specialist focused on block/parry skill support).
- **Relationship to Arden:** doesn't know who he really is unless the player chooses
  to tell her in a specific Arc X dialogue branch; friendship-only track.

### Ashlin Rell (non-combat, guide-type companion)
- **Age:** unknown (Varinthe spirit, centuries old in subjective memory, bound to
  the Deep Archive). **Appearance:** translucent, period Varinthe dress, visible only
  within archive spaces until Arc VIII.
- **History:** a Varinthe archivist who chose to remain bound to her people's
  archive rather than let its knowledge vanish with them.
- **Motivation:** wants the Varinthe's mistakes understood, not repeated.
- **Personal quest:** Arc VI (introduction, Curseweave unlock) → Arc VIII (major
  story role) → Arc XII (final appearance, context-dependent on ending chosen).
- **Relationship to Arden:** mentor figure; not romanceable; one of the few NPCs who
  treats Arden's "soul from elsewhere" status as mundane rather than shocking.

---

## Family & Hometown NPCs

### Yulia Calt
- **Age:** 41. Joren's mother; laborer, fiercely protective, increasingly aware
  something about her son changed after the Trial of Awakening even if she can't
  name it. Recurring quest-giver through Arc IV; letters from her appear through
  Arcs VI–IX as the stakes rise.

### Pip Calt
- **Age:** 11 → ages visibly across the game's ~10-month content arc structure.
  Joren's sister; Rank D Aptitude; her own ambition thread (wanting to matter)
  pays off with a cameo at Rank B Aptitude by Arc XI, explicitly because she kept
  training rather than because of anything Arden did for her — a deliberate contrast
  to power-by-proximity.

### Overseer Brennick Oss
- **Age:** 52. Dominion aptitude inspector, antagonist-adjacent comic/petty-bully
  figure in Arc II, reappears briefly in Arc IV with a demotion of his own after
  his testing methodology is publicly questioned (minor, optional payoff thread).

### Dane Oshiro
- **Age:** 58. Retired Dominion soldier, Veyth Hollow general-store owner and
  Arden's first combat trainer; recurring vendor/trainer across the whole game via
  a simple shop-restock system, not a one-off NPC.

---

## Faction Figures

### Director Ansel Vorne
- **Age:** 61. Head of the Dominion's Anomaly Containment Directorate; the human
  institutional antagonist of Arcs X–XII. Not a cartoon villain — believes
  unregistered anomalies are an existential risk and that extreme measures are
  justified, which the story lets the player see argued coherently even while
  opposing it.

### Inspector Calloway Brisk
- **Age:** 35. Imperial Guild inspector running an artifact-smuggling operation
  under cover of legitimate authority; Arc V/Arc X antagonist figure, defeated or
  exposed depending on branch.

### Castellan Orve Dessa
- **Age:** 49. Trade Union merchant-prince, recurring neutral-faction broker across
  Arcs IV, VII, IX.

### Noble-Marshal Edevane Thresh / Isolde Rin / Vash'tor Keel
See `Docs/Factions/00_Factions.md` for full faction-representative profiles — these
three anchor the Arc VII political branch and recur through Arc IX/X depending on
player choice.

### Mira Reyes
- **Age:** 24. Sister of Arc I's Scout Tobin Reyes; Lower Docks dockworker turned
  refugee-camp organizer; her Arc IV/Arc IX arc is the game's clearest small-scale
  consequence thread tied to a single early dialogue choice.

---

*Remaining roster entries (minimum 20 total per design brief, 11 detailed above)
continue in `Docs/Characters/01_Extended_Roster.md` — regional questgivers, guild
trainers, and faction rank-and-file figures referenced by the quest database.*
