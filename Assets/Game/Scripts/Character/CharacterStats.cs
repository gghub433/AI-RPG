using System;
using UnityEngine;
using RebirthEclipse.Core;

namespace RebirthEclipse.Character
{
    /// <summary>
    /// Runtime stat holder attached to every combat-capable actor (player, companion, enemy, boss).
    /// Owns current HP/MP/Stamina and recomputes derived stats whenever base stats, level,
    /// or equipment bonuses change. This is the single source of truth every other system
    /// (combat, UI, Eclipse System HUD) reads from — nothing else is allowed to cache its own
    /// copy of health/mana/stamina.
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Identity")]
        public string characterId;
        public bool isPlayer;

        [Header("Base Stats")]
        [SerializeField] private StatBlock baseStats = StatBlock.StartingBlock;
        [SerializeField] private int level = 1;

        private StatBlock equipmentFlatBonus; // flat stat bonus from gear (str/agi/etc.)
        private DerivedStats equipmentDerivedBonus; // direct derived-stat bonus from gear (e.g. +flat crit%)

        public DerivedStats Derived { get; private set; }

        public float CurrentHealth { get; private set; }
        public float CurrentMana { get; private set; }
        public float CurrentStamina { get; private set; }

        public bool IsDead => CurrentHealth <= 0f;

        public int Level => level;
        public StatBlock BaseStats => baseStats;
        public StatBlock EffectiveStats => baseStats + equipmentFlatBonus;

        private void Awake()
        {
            Recalculate();
            CurrentHealth = Derived.maxHealth;
            CurrentMana = Derived.maxMana;
            CurrentStamina = Derived.maxStamina;
        }

        public void Recalculate()
        {
            Derived = DerivedStats.Compute(EffectiveStats, level, equipmentDerivedBonus);
            CurrentHealth = Mathf.Min(CurrentHealth, Derived.maxHealth);
            CurrentMana = Mathf.Min(CurrentMana, Derived.maxMana);
            CurrentStamina = Mathf.Min(CurrentStamina, Derived.maxStamina);
        }

        public void SetLevel(int newLevel)
        {
            level = Mathf.Max(1, newLevel);
            Recalculate();
        }

        public void AddEquipmentBonus(StatBlock flatBonus, DerivedStats derivedBonus)
        {
            equipmentFlatBonus += flatBonus;
            equipmentDerivedBonus.maxHealth += derivedBonus.maxHealth;
            equipmentDerivedBonus.maxMana += derivedBonus.maxMana;
            equipmentDerivedBonus.maxStamina += derivedBonus.maxStamina;
            equipmentDerivedBonus.physicalPower += derivedBonus.physicalPower;
            equipmentDerivedBonus.magicPower += derivedBonus.magicPower;
            equipmentDerivedBonus.physicalDefense += derivedBonus.physicalDefense;
            equipmentDerivedBonus.magicDefense += derivedBonus.magicDefense;
            equipmentDerivedBonus.curseDefense += derivedBonus.curseDefense;
            equipmentDerivedBonus.critChance += derivedBonus.critChance;
            equipmentDerivedBonus.critDamageMult += derivedBonus.critDamageMult;
            equipmentDerivedBonus.attackSpeedMult += derivedBonus.attackSpeedMult;
            equipmentDerivedBonus.cooldownReduction += derivedBonus.cooldownReduction;
            equipmentDerivedBonus.manaCostReduction += derivedBonus.manaCostReduction;
            equipmentDerivedBonus.moveSpeed += derivedBonus.moveSpeed;
            Recalculate();
        }

        public void ResetEquipmentBonuses()
        {
            equipmentFlatBonus = StatBlock.Zero;
            equipmentDerivedBonus = default;
            Recalculate();
        }

        /// <summary>
        /// Applies damage after all mitigation has already been computed by DamageCalculator.
        /// Raises death/damage events exactly once per call — callers must not re-apply.
        /// </summary>
        public void ApplyDamage(float finalAmount, DamageType type)
        {
            if (IsDead) return;
            finalAmount = Mathf.Max(0f, finalAmount);
            CurrentHealth = Mathf.Max(0f, CurrentHealth - finalAmount);
            GameEvents.RaiseDamageDealt(this, finalAmount, type);
            if (CurrentHealth <= 0f)
            {
                GameEvents.RaiseCharacterDied(this);
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Min(Derived.maxHealth, CurrentHealth + Mathf.Max(0f, amount));
        }

        public bool TrySpendMana(float amount)
        {
            float cost = amount * (1f - Derived.manaCostReduction);
            if (CurrentMana < cost) return false;
            CurrentMana -= cost;
            return true;
        }

        public bool TrySpendStamina(float amount)
        {
            if (CurrentStamina < amount) return false;
            CurrentStamina -= amount;
            return true;
        }

        public void RegenerateOverTime(float deltaTime, float manaRegenPerSec, float staminaRegenPerSec)
        {
            if (IsDead) return;
            CurrentMana = Mathf.Min(Derived.maxMana, CurrentMana + manaRegenPerSec * deltaTime);
            CurrentStamina = Mathf.Min(Derived.maxStamina, CurrentStamina + staminaRegenPerSec * deltaTime);
        }

        /// <summary>
        /// Revive with a fraction of max health; used by the respawn / checkpoint flow.
        /// Never called automatically — the SaveSystem or a scripted event must trigger it explicitly.
        /// </summary>
        public void Revive(float healthFraction = 0.5f)
        {
            CurrentHealth = Derived.maxHealth * Mathf.Clamp01(healthFraction);
            CurrentMana = Derived.maxMana * 0.5f;
            CurrentStamina = Derived.maxStamina;
            GameEvents.RaiseCharacterRevived(this);
        }

        public void LoadFromSave(StatBlock savedBase, int savedLevel, float savedHp, float savedMp, float savedSt)
        {
            baseStats = savedBase;
            level = savedLevel;
            Recalculate();
            CurrentHealth = Mathf.Clamp(savedHp, 0f, Derived.maxHealth);
            CurrentMana = Mathf.Clamp(savedMp, 0f, Derived.maxMana);
            CurrentStamina = Mathf.Clamp(savedSt, 0f, Derived.maxStamina);
        }

        public void AllocateStatPoint(string statName, int amount = 1)
        {
            switch (statName.ToLowerInvariant())
            {
                case "strength": baseStats.strength += amount; break;
                case "agility": baseStats.agility += amount; break;
                case "vitality": baseStats.vitality += amount; break;
                case "intellect": baseStats.intellect += amount; break;
                case "spirit": baseStats.spirit += amount; break;
                case "manacontrol": baseStats.manaControl += amount; break;
                case "curseresistance": baseStats.curseResistance += amount; break;
                case "luck": baseStats.luck += amount; break;
                default: Debug.LogWarning($"Unknown stat name: {statName}"); return;
            }
            Recalculate();
        }
    }
}
