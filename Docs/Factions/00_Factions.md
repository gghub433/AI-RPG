# Factions

Seven power blocs, each with concrete goals, resources, inter-faction relationships,
and story influence — not interchangeable "good guild / bad guild" labels.

## Imperial Guild
**Goal:** monopolize licensed mercenary/contractor work across Dominion territory.
**Resources:** the bounty-contract economy (Arc IV's repeatable-quest hub),
Aptitude-rank licensing authority. **Relationships:** nominally allied with the
Thresh Dominion but self-interested; quietly tolerates Brisk's smuggling ring until
exposed. **Player influence:** Guild Rank track (reputation unlocks better
contracts/vendor prices, never gates main story).

## Order of Ash
**Goal:** monastic order dedicated to "burning out" Curse-corruption wherever it's
found — well-intentioned, occasionally fanatical in method. **Resources:** the
Curseweave counter-lore to Arc VI's Varinthe material (Order doctrine and Varinthe
truth disagree, intentionally, giving players two conflicting in-world
explanations for Curse damage to weigh). **Relationships:** suspicious of the
Silver Moon Clan's anomaly-energy caution, seeing it as insufficiently thorough;
cordial with the Dominion. **Player influence:** optional faction questline
(post-Arc VI) offering Curse-resistance gear exclusive to Order reputation.

## Clan of the Silver Moon
**Goal:** contain and ultimately seal all anomaly-derived energy sources,
permanently. **Resources:** old, disciplined combat tradition; deep institutional
memory of past anomaly disasters. **Relationships:** historic debt-ties to several
minor Dominion houses (Kael Thorne's family among them); wary but not hostile
toward the Demonic Houses. **Player influence:** Arc VII political branch; Isolde
Rin as recurring contact.

## Trade Union
**Goal:** keep trade routes open and profitable regardless of who's technically in
charge politically. **Resources:** the broadest material/crafting supply network in
the game; Castellan Dessa's broker relationships with every other faction.
**Relationships:** deliberately non-aligned; the only faction that stays
functionally neutral through the Arc IX siege. **Player influence:** crafting
material access track, independent of story branching.

## Demonic Houses
**Goal:** modernize their political standing away from old monstrous stereotyping,
primarily through becoming indispensable trade/power brokers. **Resources:**
Vash'tor Keel's broker network; House-specific combat traditions distinct from
Dominion/Clan styles (a design note for future enemy/companion variety). **Relationships:**
tense-but-functional with the Dominion; see the Silver Moon Clan as overly rigid.
**Player influence:** Arc VII political branch; unique black-market vendor access.

## Thresh Dominion (Anomaly Containment Directorate)
**Goal:** officially, regional stability and lawful Aptitude licensing; covertly
(via Director Vorne's Directorate), total suppression of unregistered anomalies
including, eventually, Arden himself. **Resources:** the Aptitude testing/licensing
system that frames Arden's entire "Rank F" underestimation arc; the largest
standing military force in the setting. **Relationships:** Vorne's black-budget
authority creates an internal faction conflict (Thresh the politician vs. Vorne the
Directorate) the player can exploit in Arc X. **Player influence:** Arc VII/IX/X/XII
critical-path faction; cannot be "destroyed," only politically outmaneuvered or
reconciled with.

## Cult of the Hollow Depths
**Goal:** force open a Varinthe transfer gate to retrieve the System's original
"true anchor soul" and control the Eclipse System directly. **Resources:**
fragmentary Varinthe ritual knowledge, recruited true believers across multiple
regions (not a monolithic evil army — cells appear as early as Arc VI as minor
patrol enemies before the group is named in Arc IX). **Relationships:** opposed to
everyone, opportunistic about exploiting the Dominion/Clan/Houses' Arc VII conflict.
**Player influence:** Arc IX–XII primary antagonist faction.

---

## Faction Relationship Summary

| Faction | Dominion | Order of Ash | Silver Moon | Trade Union | Demonic Houses | Cult |
|---|---|---|---|---|---|---|
| Dominion | — | Cordial | Tense | Allied | Tense | Hostile |
| Order of Ash | Cordial | — | Suspicious-of | Cordial | Neutral | Hostile |
| Silver Moon | Tense | Suspicious-of | — | Cordial | Wary | Hostile |
| Trade Union | Allied | Cordial | Cordial | — | Allied | Hostile |
| Demonic Houses | Tense | Neutral | Wary | Allied | — | Hostile |
| Cult | Hostile | Hostile | Hostile | Hostile | Hostile | — |

Reputation with each faction is tracked independently (see
`RebirthEclipse.Relationships` for the NPC-level affinity system this scales from at
a faction level in a later implementation pass) and influences vendor pricing,
available side-quests, and which NPCs greet the player as an ally vs. a stranger.
