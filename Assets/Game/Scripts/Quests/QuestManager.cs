using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.Progression;

namespace RebirthEclipse.Quests
{
    [System.Serializable]
    public class ActiveQuestState
    {
        public string questId;
        public Dictionary<string, int> objectiveProgress = new Dictionary<string, int>();
        public bool isCompleted;
    }

    /// <summary>
    /// Tracks active/completed quests and the persistent story-flag set. Objective progress
    /// is driven exclusively by GameEvents — kills, pickups, dialogue outcomes — so a quest
    /// can never silently complete from UI interaction alone. Also owns the hidden-quest
    /// discovery log referenced by the Eclipse System's "journal of discovered secrets".
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestDatabase questDatabase;
        [SerializeField] private LevelingSystem playerLeveling;

        private readonly Dictionary<string, ActiveQuestState> activeQuests = new Dictionary<string, ActiveQuestState>();
        private readonly HashSet<string> completedQuestIds = new HashSet<string>();
        private readonly HashSet<string> storyFlags = new HashSet<string>();

        public IReadOnlyCollection<string> StoryFlags => storyFlags;
        public IReadOnlyDictionary<string, ActiveQuestState> ActiveQuests => activeQuests;
        public IReadOnlyCollection<string> CompletedQuests => completedQuestIds;

        private void OnEnable()
        {
            GameEvents.OnCharacterDied += HandleEnemyKilled;
            GameEvents.OnItemAcquired += HandleItemAcquired;
        }

        private void OnDisable()
        {
            GameEvents.OnCharacterDied -= HandleEnemyKilled;
            GameEvents.OnItemAcquired -= HandleItemAcquired;
        }

        public bool CanStartQuest(QuestDefinition quest)
        {
            if (quest == null) return false;
            if (completedQuestIds.Contains(quest.questId) && quest.category != QuestCategory.Repeatable) return false;
            if (activeQuests.ContainsKey(quest.questId)) return false;
            if (playerLeveling != null && playerLeveling.CurrentLevel < quest.minimumLevel) return false;
            if (quest.prerequisiteStoryFlags != null)
                foreach (var flag in quest.prerequisiteStoryFlags)
                    if (!storyFlags.Contains(flag)) return false;
            if (quest.prerequisiteQuestIds != null)
                foreach (var qId in quest.prerequisiteQuestIds)
                    if (!completedQuestIds.Contains(qId)) return false;
            return true;
        }

        public bool StartQuest(string questId)
        {
            var def = questDatabase.Find(questId);
            if (!CanStartQuest(def)) return false;

            var state = new ActiveQuestState { questId = questId };
            foreach (var obj in def.objectives) state.objectiveProgress[obj.objectiveId] = 0;
            activeQuests[questId] = state;

            if (def.category == QuestCategory.HiddenQuest)
            {
                EclipseSystemModel.Instance?.LogSecret($"hidden_quest_discovered_{questId}");
            }

            GameEvents.RaiseQuestStarted(questId);
            return true;
        }

        private void HandleEnemyKilled(Character.CharacterStats who)
        {
            if (who == null || who.isPlayer) return;
            AdvanceObjectives(ObjectiveType.KillTarget, who.characterId, 1);
        }

        private void HandleItemAcquired(string itemId, int quantity)
        {
            AdvanceObjectives(ObjectiveType.CollectItem, itemId, quantity);
        }

        public void ReportReachLocation(string locationId) => AdvanceObjectives(ObjectiveType.ReachLocation, locationId, 1);
        public void ReportTalkToNpc(string npcId) => AdvanceObjectives(ObjectiveType.TalkToNpc, npcId, 1);
        public void ReportAbilityUsed(string abilityId) => AdvanceObjectives(ObjectiveType.UseAbility, abilityId, 1);

        private void AdvanceObjectives(ObjectiveType type, string targetId, int amount)
        {
            foreach (var kv in activeQuests.ToList())
            {
                if (kv.Value.isCompleted) continue;
                var def = questDatabase.Find(kv.Key);
                if (def == null) continue;

                bool changed = false;
                foreach (var obj in def.objectives)
                {
                    if (obj.type != type || obj.targetId != targetId) continue;
                    int current = kv.Value.objectiveProgress[obj.objectiveId];
                    if (current >= obj.requiredCount) continue;
                    kv.Value.objectiveProgress[obj.objectiveId] = Mathf.Min(obj.requiredCount, current + amount);
                    changed = true;
                    GameEvents.RaiseQuestObjectiveUpdated(def.questId, obj.objectiveId);
                }

                if (changed) TryCompleteQuest(def, kv.Value);
            }
        }

        private void TryCompleteQuest(QuestDefinition def, ActiveQuestState state)
        {
            foreach (var obj in def.objectives)
            {
                if (obj.isOptional) continue;
                if (state.objectiveProgress[obj.objectiveId] < obj.requiredCount) return; // not all required objectives met
            }

            state.isCompleted = true;
            activeQuests.Remove(def.questId);
            completedQuestIds.Add(def.questId);

            if (def.reward != null)
            {
                if (def.reward.experience > 0) playerLeveling?.GrantExperience(def.reward.experience);
                if (!string.IsNullOrEmpty(def.reward.unlockStoryFlag)) SetStoryFlag(def.reward.unlockStoryFlag);
                // Item rewards are granted by the caller (QuestCompletionHandler) which has
                // access to InventorySystem — kept out of QuestManager to avoid a circular
                // dependency between the two systems.
            }

            GameEvents.RaiseQuestCompleted(def.questId);
        }

        public void SetStoryFlag(string flagId)
        {
            if (storyFlags.Add(flagId))
            {
                GameEvents.RaiseStoryFlagSet(flagId);
            }
        }

        public bool HasStoryFlag(string flagId) => storyFlags.Contains(flagId);

        public void LoadFromSave(IEnumerable<string> savedFlags, IEnumerable<string> savedCompleted, List<ActiveQuestState> savedActive)
        {
            storyFlags.Clear();
            foreach (var f in savedFlags) storyFlags.Add(f);
            completedQuestIds.Clear();
            foreach (var c in savedCompleted) completedQuestIds.Add(c);
            activeQuests.Clear();
            foreach (var a in savedActive) activeQuests[a.questId] = a;
        }
    }
}
