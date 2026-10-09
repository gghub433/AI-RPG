# Classes

Eight classes, each with identity, strengths/weaknesses, evolution stages, and a
Guild Trainer NPC for flavor/respec dialogue. Implemented as `ClassDefinition`
ScriptableObject assets (`Assets/Game/Scripts/Progression/ClassDefinition.cs`);
this document is the design source those assets are authored from.

## 1. Теневой клинок (Shadow Blade)
**Identity:** high-mobility single-target melee, crit-focused.
**Strengths:** burst damage, dodge uptime, single-target duels.
**Weaknesses:** low AoE, fragile against sustained pressure.
**Evolution:** Stage 1 "Shadow Blade" (Lv.1) → Stage 2 "Duskfang" (Lv.15,
flag `arc4_serah_recruited` — Serah's investigator instincts rub off) →
Stage 3 "Blade of the Severed Dusk" (Lv.35, flag `arc8_system_truth_revealed`).
**Guild Trainer:** Issa Kol, ex-duelist.

## 2. Рыцарь Бездны (Abyss Knight)
**Identity:** tanky melee, built around Abyss-energy-fueled defensive cooldowns.
**Strengths:** sustain, party protection, Curse resistance synergy.
**Weaknesses:** low mobility, mana-hungry defensive kit.
**Evolution:** Stage 1 "Abyss Knight" (Lv.1) → Stage 2 "Hollowguard" (Lv.15, flag
`arc6_curseweave_unlocked`) → Stage 3 "Warden of the Unanchored" (Lv.35, flag
`arc11_cult_goal_revealed`).
**Guild Trainer:** Garrick Voss, retired Order of Ash penitent.

## 3. Маг пространственных печатей (Spatial Seal Mage)
**Identity:** zone-control caster — seals/wards that reposition enemies and
project damage fields.
**Strengths:** crowd control, strong vs. groups.
**Weaknesses:** weak single-target burst, setup time.
**Evolution:** Stage 1 → Stage 2 "Sealwright" (Lv.15, flag `arc3_fragment_warden_found`)
→ Stage 3 "Seal of the First Vault" (Lv.35, flag `arc8_hollow_reach_recontextualized`).
**Guild Trainer:** Ophel Dray-Kess (distant cousin of Mireille Dray).

## 4. Пожиратель энергии (Energy Eater)
**Identity:** mana-drain hybrid, converts enemy resources into player sustain.
**Strengths:** excellent resource sustain, strong in long fights.
**Weaknesses:** weak burst, punished by enemies with low mana pools.
**Evolution:** Stage 1 → Stage 2 "Hungering Current" (Lv.15, requires 5 total
Abyss absorptions performed) → Stage 3 "Devourer of the Hollow" (Lv.35, flag
`arc6_fragment_varinthe_origin`).
**Guild Trainer:** Resh Talmadge.

## 5. Призыватель осколков (Shard Summoner)
**Identity:** pet/summon class using "shards" of previously scanned (not
necessarily absorbed) enemies to field temporary allies.
**Strengths:** action economy, split damage sources.
**Weaknesses:** micromanagement-heavy, summons are individually fragile.
**Evolution:** Stage 1 → Stage 2 "Shardbinder" (Lv.15, requires 10 unique enemies
scanned) → Stage 3 "Caller of the Buried Choir" (Lv.35, flag `arc9_capital_fallen_districts` non-empty).
**Guild Trainer:** Nyla Second-Voice.

## 6. Хранитель душ (Soulkeeper)
**Identity:** support/healer hybrid built around Sync Rate synergy — several
Soulkeeper skills scale directly off the Eclipse Sync Rate stat, the game's
clearest mechanical tie-in between the narrative System and a class kit.
**Strengths:** party sustain, unique Sync-scaling buffs.
**Weaknesses:** low personal damage.
**Evolution:** Stage 1 → Stage 2 "Anchor-Tender" (Lv.15, Sync Rate ≥ 25%) →
Stage 3 "Keeper of the Unbroken Line" (Lv.35, Sync Rate ≥ 60%).
**Guild Trainer:** Ashlin Rell herself, for players who pursue this class (unique
trainer dialogue overlap with her Arc VI/VIII story role).

## 7. Разрушитель измерений (Dimension Breaker)
**Identity:** high-risk burst melee/caster hybrid unlocked around the Arc XI
rift content; skills interact with environmental rift-instability mechanics.
**Strengths:** highest raw burst potential in the game.
**Weaknesses:** self-damage risk on missed timing windows, late unlock.
**Evolution:** Stage 1 unlocks at Lv.30 + flag `arc11_cult_goal_revealed` (no
earlier stage — this is an Arc XI-onward class by design) → Stage 2 "Breaker of
the Still Hollow" (Lv.45, flag `arc11_still_hollow_witnessed`).
**Guild Trainer:** none — self-taught via a Rift Expanse shrine, reinforcing its
outsider identity.

## 8. Секретный класс: Абсолютная Бездна (Absolute Abyss — Hidden Class)
**Identity:** the signature hidden class referenced throughout the design brief.
**Unlock requirements (all required, not a simple level gate):**
1. Eclipse Sync Rate ≥ 80% (requires substantial absorption + story-flag progress).
2. At least 3 of the 4 "optional lore fragment" absorptions completed
   (`fragment_warden_ashgrave`, `fragment_varinthe_origin`, plus two more
   introduced in Arc IX/X documents as implementation proceeds).
3. Flag `arc8_system_truth_revealed` set.
4. Reached Arc XII's Source Chamber.
**Strengths:** access to every other class's Stage-1 skill as a situational swap,
plus a unique ultimate tied to the ending chosen in Arc XII.
**Weaknesses:** deliberately the most mechanically demanding class — low
forgiveness for resource mismanagement, by design, as the "mastery reward" class.
**Evolution:** single stage — this is an endgame-only unlock, not a long-term
growth path like the other seven.
**Guild Trainer:** none; the Source itself is the only source of training dialogue
for this class.

---

## Skill Point Economy
- 1 skill point per level (see `LevelingSystem.skillPointsPerLevel`).
- Respec available at any Guild Trainer for a scaling currency cost, or free once
  per arc via a "Memory Thread" rare consumable dropped from mini-bosses.
- Skill trees are fully data-driven (`SkillTreeDefinition` assets); adding a ninth
  class or rebalancing an existing tree requires no code changes.
