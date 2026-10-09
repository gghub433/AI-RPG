using System;
using UnityEngine;

namespace RebirthEclipse.Character
{
    /// <summary>
    /// The eight primary attributes. Pure data — no Unity lifecycle, so it can be
    /// serialized straight into save files and unit-tested without a scene.
    /// </summary>
    [Serializable]
    public struct StatBlock
    {
        public int strength;         // physical damage, carry weight
        public int agility;          // attack speed, dodge chance, crit chance
        public int vitality;         // max health, physical resistance
        public int intellect;        // magic damage, max mana
        public int spirit;           // mana regen, skill cooldown reduction
        public int manaControl;      // skill cost reduction, cast speed
        public int curseResistance;  // resistance to curse/void damage and debuffs
        public int luck;             // rare drop chance, crit damage, absorption success

        public static StatBlock Zero => new StatBlock();

        public static StatBlock StartingBlock => new StatBlock
        {
            strength = 8,
            agility = 8,
            vitality = 10,
            intellect = 6,
            spirit = 6,
            manaControl = 5,
            curseResistance = 4,
            luck = 5
        };

        public static StatBlock operator +(StatBlock a, StatBlock b) => new StatBlock
        {
            strength = a.strength + b.strength,
            agility = a.agility + b.agility,
            vitality = a.vitality + b.vitality,
            intellect = a.intellect + b.intellect,
            spirit = a.spirit + b.spirit,
            manaControl = a.manaControl + b.manaControl,
            curseResistance = a.curseResistance + b.curseResistance,
            luck = a.luck + b.luck
        };

        public int GetByName(string statName)
        {
            switch (statName.ToLowerInvariant())
            {
                case "strength": return strength;
                case "agility": return agility;
                case "vitality": return vitality;
                case "intellect": return intellect;
                case "spirit": return spirit;
                case "manacontrol": return manaControl;
                case "curseresistance": return curseResistance;
                case "luck": return luck;
                default: return 0;
            }
        }
    }

    /// <summary>
    /// Derived combat numbers computed from a StatBlock + level + equipment.
    /// Recomputed whenever any input changes; never hand-edited directly.
    /// </summary>
    [Serializable]
    public struct DerivedStats
    {
        public float maxHealth;
        public float maxMana;
        public float maxStamina;
        public float physicalPower;
        public float magicPower;
        public float physicalDefense;
        public float magicDefense;
        public float curseDefense;
        public float critChance;      // 0..1
        public float critDamageMult;  // multiplier, e.g. 1.5
        public float attackSpeedMult;
        public float cooldownReduction; // 0..1
        public float manaCostReduction; // 0..1
        public float moveSpeed;

        public static DerivedStats Compute(StatBlock stats, int level, DerivedStats equipmentBonus)
        {
            var d = new DerivedStats
            {
                maxHealth = 80f + stats.vitality * 12f + level * 10f,
                maxMana = 40f + stats.intellect * 8f + stats.manaControl * 2f + level * 4f,
                maxStamina = 60f + stats.agility * 6f + stats.vitality * 2f,
                physicalPower = stats.strength * 2.2f + stats.agility * 0.4f,
                magicPower = stats.intellect * 2.4f + stats.spirit * 0.5f,
                physicalDefense = stats.vitality * 1.1f + stats.strength * 0.3f,
                magicDefense = stats.spirit * 1.0f + stats.intellect * 0.3f,
                curseDefense = stats.curseResistance * 1.5f,
                critChance = Mathf.Clamp01(0.03f + stats.agility * 0.0025f + stats.luck * 0.0015f),
                critDamageMult = 1.5f + stats.luck * 0.01f,
                attackSpeedMult = 1.0f + stats.agility * 0.004f,
                cooldownReduction = Mathf.Clamp01(stats.spirit * 0.002f + stats.manaControl * 0.0015f),
                manaCostReduction = Mathf.Clamp01(stats.manaControl * 0.0025f),
                moveSpeed = 5.0f + stats.agility * 0.01f
            };

            d.maxHealth += equipmentBonus.maxHealth;
            d.maxMana += equipmentBonus.maxMana;
            d.maxStamina += equipmentBonus.maxStamina;
            d.physicalPower += equipmentBonus.physicalPower;
            d.magicPower += equipmentBonus.magicPower;
            d.physicalDefense += equipmentBonus.physicalDefense;
            d.magicDefense += equipmentBonus.magicDefense;
            d.curseDefense += equipmentBonus.curseDefense;
            d.critChance = Mathf.Clamp01(d.critChance + equipmentBonus.critChance);
            d.critDamageMult += equipmentBonus.critDamageMult;
            d.attackSpeedMult += equipmentBonus.attackSpeedMult;
            d.cooldownReduction = Mathf.Clamp01(d.cooldownReduction + equipmentBonus.cooldownReduction);
            d.manaCostReduction = Mathf.Clamp01(d.manaCostReduction + equipmentBonus.manaCostReduction);
            d.moveSpeed += equipmentBonus.moveSpeed;

            return d;
        }
    }
}
