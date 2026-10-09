using System;
using UnityEngine;

namespace RebirthEclipse.Progression
{
    public enum ClassRank { F, E, D, C, B, A, S, Secret }

    /// <summary>
    /// One evolution stage of a class (e.g. "Shadow Blade" -> "Blade of the Severed Dusk").
    /// Classes evolve through story-gated milestones, not raw level alone, matching the
    /// design requirement that growth comes from real progress rather than numbers-for-show.
    /// </summary>
    [Serializable]
    public class ClassEvolutionStage
    {
        public string stageName;
        [TextArea] public string flavorText;
        public int requiredLevel;
        public string requiredStoryFlag;     // empty = no story gate, level gate only
        public StatBoost passiveStatBoost;
        public string[] unlockedSkillIds;
        public string visualAuraEffectId;    // VFX prefab/addressable key for the aura change
    }

    [Serializable]
    public struct StatBoost
    {
        public int strength, agility, vitality, intellect, spirit, manaControl, curseResistance, luck;
    }

    /// <summary>
    /// Data-driven definition for one of the eight playable classes. New classes are added
    /// by creating a new asset from this template, never by branching code — CombatController
    /// and the skill system only ever read skill ids out of a ClassDefinition's current stage.
    /// </summary>
    [CreateAssetMenu(fileName = "NewClass", menuName = "RebirthEclipse/Class Definition")]
    public class ClassDefinition : ScriptableObject
    {
        public string classId;
        public string displayName;
        [TextArea(3, 8)] public string description;
        public ClassRank startingRank;
        public bool isHiddenClass;
        [TextArea(2, 6)] public string unlockRequirementDescription; // human-readable, for hidden classes

        [Header("Identity")]
        public string[] strengths;
        public string[] weaknesses;

        [Header("Evolution")]
        public ClassEvolutionStage[] evolutionStages;

        [Header("Skill Tree")]
        public SkillTreeDefinition skillTree;

        public ClassEvolutionStage GetStageForLevel(int level, System.Collections.Generic.HashSet<string> storyFlags)
        {
            ClassEvolutionStage current = null;
            foreach (var stage in evolutionStages)
            {
                bool levelOk = level >= stage.requiredLevel;
                bool flagOk = string.IsNullOrEmpty(stage.requiredStoryFlag) || storyFlags.Contains(stage.requiredStoryFlag);
                if (levelOk && flagOk)
                {
                    current = stage;
                }
            }
            return current;
        }
    }
}
