# Currency & Economy

Implemented by `Assets/Game/Scripts/Economy/` (`CurrencyWallet`, `Vendor`). Verified by
`EconomyTests`.

## Currencies

| Currency | Relation | Earned from |
|---|---|---|
| **Copper** | base unit | trash loot, selling, minor quests |
| **Silver** | 100 copper = 1 silver | quests, contracts |
| **Gold** | 100 silver = 1 gold | major quests, boss loot, trade |
| **Eclipse Tokens** | separate, **non-convertible** premium currency | specific content only (rare rewards, hidden quests) |

Copper/Silver/Gold are one fungible pool stored internally in copper, so denominations
can never desync. Eclipse Tokens are deliberately walled off from the convertible economy
so they cannot be farmed through ordinary money-making.

## Anti-duplication / balance guarantees
- The wallet has **no arbitrary setter** — all changes go through `AddCopper` / `TrySpend*`,
  which clamp (ignoring negative/zero adds) and guard against overflow rather than wrapping
  to a negative balance. This is the structural guard against money-dup exploits, mirroring
  the inventory's unique-instance-id guard for gear.
- Vendor stock with a finite `quantityAvailable` decrements on purchase and does not
  silently refill within a session, so a single rare item can't be bought infinitely.

## Pricing & reputation
`Vendor` scales prices with the player's standing with that merchant (via
`RelationshipManager` affinity): higher reputation lowers buy prices (up to a clamped
discount) and raises sell-back value (toward, but never reaching, parity). This gives
relationships a concrete economic payoff without letting buy/sell become a profit loop.

## Indicative price anchors (to tune in-editor)
| Thing | Rough price |
|---|---|
| Minor Health Draught | 10–25 copper |
| Common weapon | 1–3 silver |
| Rare weapon | 30–80 silver |
| Epic gear | 3–10 gold |
| Legendary gear | 25–60 gold |
| Unique/Mythical gear | gold + Eclipse Tokens, or quest-only |
| Night's stay at an inn | 5–15 copper |

Players earn money from quests, trade, monster hunting, exploration, and selling
resources. No stamina/energy gate and no pay-to-skip — the economy is a progression
texture, not a wall (per `Docs/Design/04_LongTermProgression.md`).
