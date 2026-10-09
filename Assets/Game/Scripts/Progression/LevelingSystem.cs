using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.Character;

namespace RebirthEclipse.Progression
{
    /// <summary>
    /// Owns experience/level math for one character (the player, in practice — companions
    /// use a simpler fixed-level scheme driven by story flags). Every number here is derived
    /// from a real formula, never randomized "for show": the same total XP always yields the
    /// same level, so save/load and UI stay consistent.
    /// </summary>
    [RequireComponent(typeof(CharacterStats))]
    public class LevelingSystem : MonoBehaviour
    {
        [Header("Curve")]
        [SerializeField] private int baseXpToLevel = 100;
        [SerializeField] private float xpCurveExponent = 1.35f;
        [SerializeField] private int maxLevel = 80;
        [SerializeField] private int freeStatPointsPerLevel = 5;
        [SerializeField] private int skillPointsPerLevel = 1;

        public int TotalExperience { get; private set; }
        public int CurrentLevel { get; private set; } = 1;
        public int UnspentStatPoints { get; private set; }
        public int UnspentSkillPoints { get; private set; }

        private CharacterStats stats;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
        }

        /// <summary>Total XP required to go from level (n-1) to level n.</summary>
        public int XpRequiredForLevel(int targetLevel)
        {
            if (targetLevel <= 1) return 0;
            return Mathf.RoundToInt(baseXpToLevel * Mathf.Pow(targetLevel - 1, xpCurveExponent));
        }

        public int XpRequiredForNextLevel() => XpRequiredForLevel(CurrentLevel + 1);

        public void GrantExperience(int amount)
        {
            if (amount <= 0 || CurrentLevel >= maxLevel) return;
            TotalExperience += amount;
            GameEvents.RaiseExperienceGained(amount, TotalExperience);

            bool leveledUp = false;
            while (CurrentLevel < maxLevel && TotalExperience >= XpRequiredForLevel(CurrentLevel + 1))
            {
                CurrentLevel++;
                UnspentStatPoints += freeStatPointsPerLevel;
                UnspentSkillPoints += skillPointsPerLevel;
                leveledUp = true;
                GameEvents.RaiseLevelUp(CurrentLevel);
            }

            if (leveledUp)
            {
                stats.SetLevel(CurrentLevel);
            }
        }

        public bool TrySpendStatPoint(string statName)
        {
            if (UnspentStatPoints <= 0) return false;
            UnspentStatPoints--;
            stats.AllocateStatPoint(statName, 1);
            return true;
        }

        public bool TrySpendMultipleStatPoints(string statName, int count)
        {
            if (count <= 0 || UnspentStatPoints < count) return false;
            UnspentStatPoints -= count;
            stats.AllocateStatPoint(statName, count);
            return true;
        }

        public void LoadFromSave(int totalXp, int level, int unspentStat, int unspentSkill)
        {
            TotalExperience = totalXp;
            CurrentLevel = Mathf.Clamp(level, 1, maxLevel);
            UnspentStatPoints = unspentStat;
            UnspentSkillPoints = unspentSkill;
            stats.SetLevel(CurrentLevel);
        }
    }
}
