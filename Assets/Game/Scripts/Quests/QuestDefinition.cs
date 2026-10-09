using System;
using UnityEngine;

namespace RebirthEclipse.Quests
{
    public enum ObjectiveType { KillTarget, CollectItem, ReachLocation, TalkToNpc, UseAbility, StoryFlag, SurviveTime }

    [Serializable]
    public class QuestObjective
    {
        public string objectiveId;
        [TextArea(1, 3)] public string displayText;
        public ObjectiveType type;
        public string targetId;      // enemyId / itemId / locationId / npcId / flagId depending on type
        public int requiredCount = 1;
        public bool isOptional;
        public bool isHidden; // revealed only once discovered — for "скрытые задания"
    }

    [Serializable]
    public class QuestReward
    {
        public int experience;
        public string[] itemIds;
        public int[] itemQuantities;
        public int currency;
        public string unlockStoryFlag;
    }

    public enum QuestCategory { MainStory, SideQuest, CompanionQuest, HiddenQuest, Daily, Weekly, Repeatable }

    /// <summary>
    /// Data-driven quest. Objectives are evaluated by QuestManager against real game events
    /// (kills, item pickups, story flags) — never auto-completed by a timer or by opening
    /// the quest log, satisfying the "no completion without real conditions" rule.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuest", menuName = "RebirthEclipse/Quest Definition")]
    public class QuestDefinition : ScriptableObject
    {
        public string questId;
        public string displayName;
        [TextArea(3, 10)] public string summary;
        public QuestCategory category;
        public string[] prerequisiteStoryFlags;
        public string[] prerequisiteQuestIds;
        public int minimumLevel;

        public QuestObjective[] objectives;
        public QuestReward reward;

        [Header("Branching")]
        public string[] possibleOutcomeFlags; // e.g. "sided_with_guild", "sided_with_rebels"
    }
}
