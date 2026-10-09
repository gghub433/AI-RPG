using System;
using UnityEngine;

namespace RebirthEclipse.Progression
{
    public enum SkillType { ActiveAbility, Passive, Ultimate }

    [Serializable]
    public class SkillNodeDefinition
    {
        public string skillId;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public SkillType type;
        public Sprite icon;

        [Header("Cost / Cooldown")]
        public float manaCost;
        public float staminaCost;
        public float cooldownSeconds;

        [Header("Effect")]
        public float basePower;         // scales with physicalPower or magicPower depending on scalesWithMagic
        public bool scalesWithMagic;
        public Core.DamageType damageType;
        public string statusEffectId;   // optional, empty = none
        public float statusEffectChance;

        [Header("Tree Position")]
        public string[] prerequisiteSkillIds;
        public int skillPointCost = 1;
        public Vector2 treePosition;    // for UI layout only
    }

    /// <summary>
    /// A full skill tree for one class. Nodes reference each other by id (not by direct
    /// object reference) so trees can be edited and re-wired entirely from the Inspector/data
    /// without touching code, satisfying the "add skills through data" architecture requirement.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkillTree", menuName = "RebirthEclipse/Skill Tree")]
    public class SkillTreeDefinition : ScriptableObject
    {
        public string treeId;
        public SkillNodeDefinition[] nodes;

        public SkillNodeDefinition GetNode(string skillId)
        {
            foreach (var n in nodes)
                if (n.skillId == skillId) return n;
            return null;
        }

        public bool ArePrerequisitesMet(string skillId, System.Collections.Generic.HashSet<string> unlockedSkillIds)
        {
            var node = GetNode(skillId);
            if (node == null) return false;
            if (node.prerequisiteSkillIds == null || node.prerequisiteSkillIds.Length == 0) return true;
            foreach (var prereq in node.prerequisiteSkillIds)
            {
                if (!unlockedSkillIds.Contains(prereq)) return false;
            }
            return true;
        }
    }
}
