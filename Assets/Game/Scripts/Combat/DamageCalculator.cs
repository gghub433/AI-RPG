using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Core;

namespace RebirthEclipse.Combat
{
    /// <summary>
    /// Pure, stateless damage math shared by every attack in the game — basic hits, skills,
    /// boss telegraphs. Kept free of MonoBehaviour so it is trivially unit-testable.
    /// </summary>
    public static class DamageCalculator
    {
        public struct Result
        {
            public float finalDamage;
            public bool isCritical;
        }

        public static Result Compute(CharacterStats attacker, CharacterStats defender, float basePower, bool scalesWithMagic, DamageType type, float randomRoll01)
        {
            float power = scalesWithMagic ? attacker.Derived.magicPower : attacker.Derived.physicalPower;
            float raw = basePower + power;

            float defense = type switch
            {
                DamageType.Physical => defender.Derived.physicalDefense,
                DamageType.Magical => defender.Derived.magicDefense,
                DamageType.Curse => defender.Derived.curseDefense,
                DamageType.Void => defender.Derived.curseDefense * 0.5f + defender.Derived.magicDefense * 0.5f,
                DamageType.True => 0f,
                _ => 0f
            };

            // Diminishing-returns mitigation curve: defense reduces damage but never to zero.
            float mitigation = defense / (defense + 100f);
            float mitigated = raw * (1f - Mathf.Clamp(mitigation, 0f, 0.85f));

            bool crit = type != DamageType.True && randomRoll01 < attacker.Derived.critChance;
            float final = crit ? mitigated * attacker.Derived.critDamageMult : mitigated;

            return new Result { finalDamage = Mathf.Max(1f, final), isCritical = crit };
        }
    }
}
