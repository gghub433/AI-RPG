using System;

namespace RebirthEclipse.Core
{
    /// <summary>
    /// Central, static event bus. Keeps gameplay systems decoupled: UI, audio, and
    /// quest/story logic subscribe here instead of holding direct references to each other.
    /// Every event here must be raised from a real game-state change — never fired just to
    /// drive a UI animation with no underlying data behind it.
    /// </summary>
    public static class GameEvents
    {
        // Progression
        public static event Action<int, int> OnExperienceGained;      // (amountGained, newTotalXp)
        public static event Action<int> OnLevelUp;                    // (newLevel)
        public static event Action<string> OnClassAssigned;           // (classId)
        public static event Action<string, int> OnClassEvolutionStage; // (classId, stageIndex)
        public static event Action<float> OnSyncRateChanged;          // (newSyncRate 0..1)

        // Combat
        public static event Action<Character.CharacterStats, float, DamageType> OnDamageDealt;
        public static event Action<Character.CharacterStats> OnCharacterDied;
        public static event Action<Character.CharacterStats> OnCharacterRevived;
        public static event Action<string> OnBossPhaseChanged;        // (bossId:phaseIndex encoded by caller)

        // Abyss absorption
        public static event Action<string> OnAbsorptionAttempted;     // (targetId)
        public static event Action<string, bool> OnAbsorptionResolved; // (targetId, success)
        public static event Action<string> OnMemoryFragmentUnlocked;  // (fragmentId)

        // Inventory / Items
        public static event Action<string, int> OnItemAcquired;       // (itemId, quantity)
        public static event Action<string> OnItemEquipped;            // (itemId)

        // Quests
        public static event Action<string> OnQuestStarted;            // (questId)
        public static event Action<string, string> OnQuestObjectiveUpdated; // (questId, objectiveId)
        public static event Action<string> OnQuestCompleted;          // (questId)
        public static event Action<string> OnStoryFlagSet;            // (flagId)

        // Relationships
        public static event Action<string, int> OnAffinityChanged;    // (npcId, newAffinity)
        public static event Action<string> OnRomanceUnlocked;         // (npcId)

        // Save system
        public static event Action<int> OnGameSaved;                  // (slotIndex)
        public static event Action<int> OnGameLoaded;                 // (slotIndex)

        // --- Raise helpers (keeps null-check boilerplate out of callers) ---
        public static void RaiseExperienceGained(int gained, int total) => OnExperienceGained?.Invoke(gained, total);
        public static void RaiseLevelUp(int newLevel) => OnLevelUp?.Invoke(newLevel);
        public static void RaiseClassAssigned(string classId) => OnClassAssigned?.Invoke(classId);
        public static void RaiseClassEvolutionStage(string classId, int stage) => OnClassEvolutionStage?.Invoke(classId, stage);
        public static void RaiseSyncRateChanged(float value) => OnSyncRateChanged?.Invoke(value);

        public static void RaiseDamageDealt(Character.CharacterStats target, float amount, DamageType type) =>
            OnDamageDealt?.Invoke(target, amount, type);
        public static void RaiseCharacterDied(Character.CharacterStats who) => OnCharacterDied?.Invoke(who);
        public static void RaiseCharacterRevived(Character.CharacterStats who) => OnCharacterRevived?.Invoke(who);
        public static void RaiseBossPhaseChanged(string encoded) => OnBossPhaseChanged?.Invoke(encoded);

        public static void RaiseAbsorptionAttempted(string targetId) => OnAbsorptionAttempted?.Invoke(targetId);
        public static void RaiseAbsorptionResolved(string targetId, bool success) => OnAbsorptionResolved?.Invoke(targetId, success);
        public static void RaiseMemoryFragmentUnlocked(string fragmentId) => OnMemoryFragmentUnlocked?.Invoke(fragmentId);

        public static void RaiseItemAcquired(string itemId, int qty) => OnItemAcquired?.Invoke(itemId, qty);
        public static void RaiseItemEquipped(string itemId) => OnItemEquipped?.Invoke(itemId);

        public static void RaiseQuestStarted(string questId) => OnQuestStarted?.Invoke(questId);
        public static void RaiseQuestObjectiveUpdated(string questId, string objectiveId) => OnQuestObjectiveUpdated?.Invoke(questId, objectiveId);
        public static void RaiseQuestCompleted(string questId) => OnQuestCompleted?.Invoke(questId);
        public static void RaiseStoryFlagSet(string flagId) => OnStoryFlagSet?.Invoke(flagId);

        public static void RaiseAffinityChanged(string npcId, int value) => OnAffinityChanged?.Invoke(npcId, value);
        public static void RaiseRomanceUnlocked(string npcId) => OnRomanceUnlocked?.Invoke(npcId);

        public static void RaiseGameSaved(int slot) => OnGameSaved?.Invoke(slot);
        public static void RaiseGameLoaded(int slot) => OnGameLoaded?.Invoke(slot);
    }

    public enum DamageType
    {
        Physical,
        Magical,
        Curse,
        Void,
        True
    }
}
