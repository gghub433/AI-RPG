using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Core;
using RebirthEclipse.Inventory;
using RebirthEclipse.Progression;
using RebirthEclipse.Quests;
using RebirthEclipse.Relationships;
using RebirthEclipse.Abyss;

namespace RebirthEclipse.SaveSystem
{
    /// <summary>
    /// The one scene-level object that knows about every gameplay manager AND SaveManager.
    /// Builds a SaveData snapshot by pulling from each system's own export method, and
    /// restores a loaded SaveData by pushing into each system's own LoadFromSave method —
    /// no manager reaches into another manager's private state, and none of them know about
    /// SaveManager/JSON directly.
    /// </summary>
    public class SaveController : MonoBehaviour
    {
        [SerializeField] private CharacterStats playerStats;
        [SerializeField] private LevelingSystem leveling;
        [SerializeField] private InventorySystem inventory;
        [SerializeField] private EquipmentManager equipment;
        [SerializeField] private QuestManager quests;
        [SerializeField] private RelationshipManager relationships;
        [SerializeField] private EclipseSystemModel eclipseSystem;
        [SerializeField] private AbyssAbsorptionSystem abyss;
        [SerializeField] private World.RegionProgressTracker regionProgress;

        [SerializeField] private float autosaveIntervalSeconds = 180f;
        private float autosaveTimer;
        private double playtimeAccumulator;

        private void OnEnable()
        {
            GameEvents.OnLevelUp += HandleAutosaveTrigger;
            GameEvents.OnQuestCompleted += HandleAutosaveTrigger;
        }

        private void OnDisable()
        {
            GameEvents.OnLevelUp -= HandleAutosaveTrigger;
            GameEvents.OnQuestCompleted -= HandleAutosaveTrigger;
        }

        private void HandleAutosaveTrigger(int _) => DoAutosave();
        private void HandleAutosaveTrigger(string _) => DoAutosave();

        private void Update()
        {
            playtimeAccumulator += Time.unscaledDeltaTime;
            autosaveTimer += Time.unscaledDeltaTime;
            if (autosaveTimer >= autosaveIntervalSeconds)
            {
                autosaveTimer = 0f;
                DoAutosave();
            }
        }

        public void DoAutosave() => SaveManager.Autosave(BuildSnapshot("autosave"));

        public void SaveToSlot(int slot) => SaveManager.Save(slot, BuildSnapshot($"slot_{slot}"));

        public bool LoadFromSlot(int slot)
        {
            var data = SaveManager.Load(slot);
            if (data == null) return false;
            ApplySnapshot(data);
            return true;
        }

        public bool LoadAutosave()
        {
            var data = SaveManager.LoadAutosave();
            if (data == null) return false;
            ApplySnapshot(data);
            return true;
        }

        private SaveData BuildSnapshot(string slotName)
        {
            var data = new SaveData
            {
                saveSlotName = slotName,
                totalPlaytimeSeconds = playtimeAccumulator,
                baseStats = playerStats.BaseStats,
                level = leveling.CurrentLevel,
                totalExperience = leveling.TotalExperience,
                unspentStatPoints = leveling.UnspentStatPoints,
                unspentSkillPoints = leveling.UnspentSkillPoints,
                currentHealth = playerStats.CurrentHealth,
                currentMana = playerStats.CurrentMana,
                currentStamina = playerStats.CurrentStamina,
                inventoryStacks = inventory.ExportForSave(),
                equippedBySlot = equipment.ExportForSave().ToDictionary(kv => kv.Key.ToString(), kv => kv.Value),
                storyFlags = quests.StoryFlags.ToList(),
                completedQuestIds = quests.CompletedQuests.ToList(),
                activeQuests = quests.ActiveQuests.Select(kv => new SerializableActiveQuest
                {
                    questId = kv.Key,
                    objectiveIds = kv.Value.objectiveProgress.Keys.ToList(),
                    objectiveProgress = kv.Value.objectiveProgress.Values.ToList()
                }).ToList(),
                npcAffinity = relationships.ExportForSave(),
                syncRate = eclipseSystem.SyncRate,
                scannedEnemyIds = eclipseSystem.ScannedEnemies.ToList(),
                secretsLog = eclipseSystem.SecretsLog.ToList(),
                abyssEnergy = abyss.CurrentEnergy,
                unlockedMemoryFragments = abyss.UnlockedMemoryFragments.ToList(),
                unlockedRegionIds = regionProgress != null ? regionProgress.UnlockedRegionIds.ToList() : new List<string>(),
                unlockedFastTravelPointIds = regionProgress != null ? regionProgress.UnlockedFastTravelIds.ToList() : new List<string>()
            };
            return data;
        }

        private void ApplySnapshot(SaveData data)
        {
            playerStats.LoadFromSave(data.baseStats, data.level, data.currentHealth, data.currentMana, data.currentStamina);
            leveling.LoadFromSave(data.totalExperience, data.level, data.unspentStatPoints, data.unspentSkillPoints);
            inventory.LoadFromSave(data.inventoryStacks);

            // EquipmentManager.LoadFromSave expects Dictionary<EquipmentSlot, string>; the save
            // file stores slot names as strings so SaveData has no Unity/gameplay enum dependency.
            var typedEquip = new Dictionary<EquipmentSlot, string>();
            foreach (var kv in data.equippedBySlot)
            {
                if (System.Enum.TryParse<EquipmentSlot>(kv.Key, out var slotEnum))
                    typedEquip[slotEnum] = kv.Value;
            }
            equipment.LoadFromSave(typedEquip);

            var activeQuestStates = data.activeQuests.Select(a =>
            {
                var state = new ActiveQuestState { questId = a.questId };
                for (int i = 0; i < a.objectiveIds.Count; i++)
                    state.objectiveProgress[a.objectiveIds[i]] = a.objectiveProgress[i];
                return state;
            }).ToList();
            quests.LoadFromSave(data.storyFlags, data.completedQuestIds, activeQuestStates);

            relationships.LoadFromSave(data.npcAffinity, data.romanceUnlockedNpcIds);
            eclipseSystem.LoadFromSave(data.syncRate, data.scannedEnemyIds, data.secretsLog, data.storyFlags);

            var resistances = new Dictionary<DamageType, float>();
            foreach (var kv in data.abyssResistances)
                if (System.Enum.TryParse<DamageType>(kv.Key, out var dt)) resistances[dt] = kv.Value;
            abyss.LoadFromSave(data.abyssEnergy, resistances, data.unlockedMemoryFragments, data.absorbedEnemyInstanceIds);

            playtimeAccumulator = data.totalPlaytimeSeconds;
        }
    }
}
