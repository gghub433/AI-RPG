# Canonical Cast

This is the authoritative character roster for Rebirth: Eclipse System. It supersedes
the placeholder names used in the earlier worldbuilding draft (`00_Companion_Roster.md`,
`01_Extended_Roster.md`) — see `Docs/World/00_WorldBible.md` §Reconciliation for how the
two drafts map together. Each entry lists the fields the design calls for: name, age,
appearance, voice style, personality, role, personal quest, secret, relationship to
Arden, and possible changes. All romanceable characters are explicitly adults, and
romance is gated in code by `NPCDefinition.isAdult` (hard requirement, never bypassed).

---

## Protagonist — Arden Vale

Age 23. Tall, athletic, black hair with silver streaks, cold violet eyes, several scars.
Calm, intelligent, guarded, resolute — slow to trust, fierce in protecting those who
earn it. Betrayed and killed by his own expedition, reborn in another world with access
to the ancient Abyss System of unknown origin. Four evolution stages (Reborn → Abyss
Awakening → Eclipse Sovereign → Absolute Eclipse) detailed in
`Docs/Design/05_HeroEvolution.md`; signature gear is the Eclipse Sovereign armor and the
greatsword Voidrend.

---

## Female Cast (10)

| # | Name | Age | Role / hook | Romanceable |
|---|---|---|---|---|
| 1 | **Liana Evergrace** | 24 | Elven archer, long silver hair; calm, proud, observant. Hides the truth about the ancient forests. Companion + romance track. | Yes |
| 2 | **Serafina Walter** | 25 | Knight in white-and-gold, sword-and-shield. Honest, strict, order-loyal — her loyalty is put to the test. | Yes |
| 3 | **Mirael Nox** | 23 | Dark-seal mage, black hair, violet eyes. Sarcastic, brilliant, dangerous. Studies the origin of Arden's System. | Yes |
| 4 | **Evelyn Rosen** | 26 | Trade-house heiress; politics and negotiation. Opens trade routes and economic quests. | Yes |
| 5 | **Kaela Draken** | 27 | Demon-blooded warrior, red eyes, heavy greatsword. Starts as an enemy. | Yes |
| 6 | **Elysia Frost** | 22 | Healer, blue hair, magic staff. Kind, but hides a tragic past. | Yes |
| 7 | **Vespera Morn** | 28 | Assassin in black armor. Distrustful, silent, tied to a secret organization. | Yes |
| 8 | **Ariel Sylvan** | 21 | Ruin researcher specializing in magic crystals. | Optional |
| 9 | **Nora Blackwood** | 30 | Mercenary captain; seasoned commander and potential ally. | Optional |
| 10 | **Iris Ashford** | unknown (adult) | Mysterious woman bound to the ancient System; true nature revealed near the finale. | Narrative-gated |

Each woman has a distinct model/outfit, voice style, personal quest chain, combat
skill set, preferences, and her own storyline. Romance is always optional and develops
gradually through the `RelationshipManager` affinity system.

### Expanded female profiles (voice / personality / secret / possible changes)

- **Liana Evergrace** — Voice: measured, lyrical, rarely raises it. Secret: the elven
  forests are dying from a cause tied to crystal over-harvesting she's complicit in
  concealing. Possible change: can be pushed to break with her people or to lead them.
- **Serafina Walter** — Voice: formal, clipped, duty-first. Secret: her order has been
  infiltrated/misused; her test is whether loyalty to the order outranks loyalty to
  what's right. Possible change: stays knight, becomes oathbreaker-by-conscience, or
  falls.
- **Mirael Nox** — Voice: dry, needling, delights in being the smartest in the room.
  Secret: she can read fragments of the System's code and has her own agenda for it.
  Possible change: trusted confidante or quiet rival depending on how much Arden shares.
- **Evelyn Rosen** — Voice: warm, transactional, never off-the-clock. Secret: her house's
  fortune rests on a contract that becomes untenable as the faction war escalates.
- **Kaela Draken** — Voice: blunt, combative, softens slowly. Secret: her demon blood
  carries a memory relevant to the System's origin. Possible change: enemy → rival →
  loyal companion arc.
- **Elysia Frost** — Voice: gentle, over-apologizes, flinches from conflict. Secret: a
  past healing she couldn't perform; carries survivor's guilt that her arc resolves.
- **Vespera Morn** — Voice: minimal, flat, every word deliberate. Secret: her
  organization's target list includes someone close to Arden.
- **Ariel Sylvan** — Voice: fast, curious, talks herself through problems aloud. Secret:
  found a crystal she shouldn't have and it's slowly changing her.
- **Nora Blackwood** — Voice: gruff, fair, commander's cadence. Secret: a past contract
  that puts her at odds with one of Arden's companions.
- **Iris Ashford** — Voice: ageless, serene, answers questions with questions. Secret:
  she IS (a fragment of) the System's intended anchor — the payoff of the opening
  cutscene's "true owner not established" warning.

---

## Male Cast (10)

| # | Name | Age | Role / hook |
|---|---|---|---|
| 1 | **Darian Crowe** | 25 | Arden's rival; fast swordsman, ambitious, proud. |
| 2 | **Rhayne Vald** | 29 | Arden's former commander, complicit in the original betrayal. |
| 3 | **Garret Stone** | 42 | Legendary blacksmith crafting unique weapons (crafting/upgrade hub NPC). |
| 4 | **Lucian Weir** | 35 | Spatial-seal mage who knows the history of the vanished civilization. |
| 5 | **Kain Drake** | 27 | Monster hunter, dual blades. |
| 6 | **Toren Blackford** | 48 | War veteran; trains Arden in heavy weapons. |
| 7 | **Fabian Mercer** | 31 | Rare-artifact merchant. |
| 8 | **Edric Solmir** | 55 | Human king concealing a political conspiracy. |
| 9 | **Zephyr Nightvale** | unknown | Immortal keeper of the ancient gates. |
| 10 | **Malkar** | ancient | A principal antagonist seeking to seize the System's source (see Arc XI–XII). |

### Expanded male profiles (secret / relationship to Arden / possible changes)

- **Darian Crowe** — rivalry that can mature into respect or curdle into enmity based on
  how Arden handles their duels; secret: his speed comes from a crystal bargain with a
  cost.
- **Rhayne Vald** — the in-world face of the original betrayal; his reappearance and the
  recontextualization of what really happened is a mid/late-game arc (ties to Arc X).
- **Garret Stone** — mentor-vendor; secret: forged one of the legendary weapons now in
  enemy hands and feels responsible.
- **Lucian Weir** — lore source for the crystal/System origin; can become a key ally or
  withhold crucial knowledge depending on trust.
- **Kain Drake** — pragmatic hunter; optional companion whose contracts can conflict with
  Arden's goals.
- **Toren Blackford** — heavy-weapon trainer; a steady moral anchor among the cast.
- **Fabian Mercer** — gray-market artifact dealer; useful and untrustworthy in equal
  measure.
- **Edric Solmir** — king whose conspiracy drives the faction-war arc; antagonist or
  uneasy ally depending on player choices.
- **Zephyr Nightvale** — gate-keeper tied to the dimension-war arc; ambiguous allegiance.
- **Malkar** — the ancient devourer; the cosmic-scale antagonist of the finale.

---

## Background NPCs

Townsfolk, guards, soldiers, mages, apprentices, merchants, mercenaries, and guild
members populate each region. Important ones carry name/appearance/personality/role/
schedule/dialogue and react to player decisions via story flags; generic NPCs reuse
base body models differentiated by face, hair, clothing, and accessories (per the
"no faked finished art" rule — base models are explicitly placeholders until real art
exists).
