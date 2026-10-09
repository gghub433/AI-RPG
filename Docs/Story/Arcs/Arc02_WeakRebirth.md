# Arc II — Weak Rebirth

**Scene range:** First full playable region (Veyth Hollow + surrounding Dusk Fields).
**Playtime target:** 2–3 hours. **Unlocks at end:** Arc III (Trial of Awakening).

## Narrative Goals

- Land the tonal shift: from war-zone ruin to a lived-in, poor border town. Low
  stakes on the surface, so the Eclipse System's weirdness (visible only to Arden)
  stands out.
- Establish Joren Calt's family — **mother Yulia Calt**, **younger sister Pip
  Calt (age 11)** — as people Arden now has a reason to protect, independent of the
  revenge plot.
- Teach every core system: inventory, equipment, leveling, the quest log, dialogue,
  and the first (very limited) use of Abyss absorption.
- Deliver the first real antagonist-adjacent figure: **Overseer Brennick Oss**, the
  Dominion aptitude inspector who holds authority over Joren's family's tenancy and
  treats "Rank F, unknown class" as a license to humiliate.

## Scene 1 — "An Unfamiliar Ceiling" (gameplay: first movement tutorial)

Arden wakes as Joren, mid-coughing-fit, in a one-room tenancy loft. Pip is already
awake, annoyed he's "still sick." Movement/camera tutorial is framed as Joren getting
out of bed and navigating the cramped loft — low risk, intimate scale, before the
town opens up.

First private System check (player-triggered via the HUD toggle, not forced):

```
╔════════════════════════════╗
   ECLIPSE SYSTEM
╠════════════════════════════╣
 Имя: Арден Вейл (тело: Джорен Калт)
 Уровень: 1
 Класс: Неизвестный
 Ранг: F
 Здоровье: 120/120
 Мана: 60/60
 Выносливость: 100/100
 Свободные очки: 5
 Навыки: 1
 Синхронизация: 3%
╚════════════════════════════╝
```

Arden's internal reaction (inner-voice text box, no voiced NPC) notes the mismatch:
a body the Dominion tested as mana-null is somehow carrying a System with real
numbers. First player-facing mystery hook of the new life.

## Scene 2 — "Veyth Hollow" (open hub, side quests unlock)

Town hub opens. Core NPCs introduced:

- **Yulia Calt** (mother) — quest-giver for domestic/economic side content.
- **Pip Calt** (sister) — companion-track lite; not a combat companion until Arc IV,
  but has her own small quest chain about wanting to test for Aptitude herself.
- **Overseer Brennick Oss** — antagonist-adjacent; publicly re-tests Joren for
  Aptitude as a humiliation ritual in front of the town square. The test reads
  "F — Null," same as always, because Oss's instruments test *mana channeling*, not
  whatever the Eclipse System runs on. This scene is the game's thesis statement:
  official systems cannot see Arden's real power, and that gap is the whole plot
  engine.
- **Dane Oshiro**, a retired Dominion soldier running the town's general store —
  recognizes "something off" about Joren's bearing (Arden's old combat instincts
  showing through) and becomes the source of Arden's first weapon and his first
  real combat tutorial (parry/block), framed as Dane "humoring the kid."

### Side Quests (minimum 5 required by design brief; listing the first wave)

1. **"Thin Broth"** — gather 6x Marshroot for Yulia so the family can eat better
   this week. Teaches gathering/inventory. Reward: 80 XP, 2x Minor Health Draught.
2. **"What the Null Boy Can Do"** — Pip dares Joren to kill 3 Hollow Rats in the
   Dusk Fields to prove the Overseer wrong. First combat quest outside the tutorial;
   first System enemy-scan use (scanning a Hollow Rat reveals its weakness to
   Physical damage and a flavor-text memory fragment lock, foreshadowing absorption).
   Reward: 120 XP, Rusted Shortsword (Common).
3. **"Dane's Measure"** — spar with Dane Oshiro (a scripted, unlosable combat
   tutorial encounter) to unlock Parry properly. Reward: Leather Jerkin (Common),
   unlocks Dane as a recurring vendor/trainer NPC.
4. **"The Toll Road"** — a hidden quest (not shown in the log until discovered):
   eavesdropping on two Dominion toll-guards near the east gate reveals they're
   skimming toll money. Resolving it (report them to Oss, or blackmail them for
   coin, or let it go) is the game's first morally-textured choice with no "correct"
   answer; each resolution sets a different minor flag read back in a single Arc IV
   line. Reward: 60 XP + outcome-dependent currency/relationship nudge.
5. **"Pip's Test"** — accompany Pip to a legitimate Dominion aptitude testing post.
   She tests Rank D (good, not great). Her reaction and Yulia's relief humanize the
   family further and set up Pip's recurring "I want to matter like you'll end up
   mattering" arc thread (paid off across several later arcs; she never becomes a
   combat companion but becomes a named correspondent/questgiver through Arc VII).

## Scene 3 — "First Real Use of the System" (gameplay: first absorption)

While clearing Hollow Rats for Pip's dare quest, the player gets the first prompt
to use **Abyss Absorption** on a defeated, scan-flagged Hollow Rat. Cost is small
(10 energy), success is guaranteed (tutorial-tier enemy), and the payoff is a tiny,
real resistance gain (+2 Physical resistance cap contribution) — the game is explicit
on-screen that this is a small, bounded gain, not a power spike, so the player's
mental model of the mechanic is correct from the first use.

## Scene 4 — "The Letter" (story beat, triggers Arc III)

A Dominion courier posts a town-wide notice: the regional **Trial of Awakening** —
an officially sanctioned, dangerous rite-of-passage dungeon run open to any
registered resident regardless of Aptitude rank — is being held at the **Aldenwatch
Spire** two days' walk from Veyth Hollow. Oss mockingly suggests Joren sign up "to
get it out of his system." Arden, reading an opportunity instead of an insult,
signs up for real. Scene ends on Yulia's worried blessing and Pip's fierce
"you'd better come back stronger, null boy."

## Flags Set at Arc End

- `arc2_family_established` (true)
- `arc2_dane_trained_player` (true) — gates access to the better trainer dialogue
  option in later arcs.
- `arc2_toll_road_outcome` (string: `reported` / `blackmailed` / `ignored`)
- `arc2_signed_trial` (true) — required prerequisite flag for Arc III start.

## Rewards Summary

~260 total XP available from listed quests, 2 pieces of Common gear, first
Abyss Absorption use, unlocked recurring NPCs (Dane Oshiro as trainer/vendor).
