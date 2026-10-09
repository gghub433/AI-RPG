using System;
using System.Collections.Generic;
using RebirthEclipse.Character;
using RebirthEclipse.Inventory;

namespace RebirthEclipse.SaveSystem
{
    /// <summary>
    /// Plain-data snapshot of the entire persistent game state. Every field here is something
    /// section 17 of the design spec requires persisting; nothing here is UI-only state.
    /// `saveFormatVersion` lets SaveManager run migrations when the schema changes between
    /// builds instead of refusing to load older saves outright.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int saveFormatVersion = 1;
        public string saveSlotName;
        public DateTime savedAtUtc;
        public double totalPlaytimeSeconds;

        // Character
        public StatBlock baseStats;
        public int level;
        public int totalExperience;
        public int unspentStatPoints;
        public int unspentSkillPoints;
        public float currentHealth, currentMana, currentStamina;
        public string currentClassId;
        public List<string> unlockedSkillIds = new List<string>();

        // Inventory / Equipment
        public List<ItemStack> inventoryStacks = new List<ItemStack>();
        public Dictionary<string, string> equippedBySlot = new Dictionary<string, string>(); // slot name -> instanceId

        // Quests / Story
        public List<string> storyFlags = new List<string>();
        public List<string> completedQuestIds = new List<string>();
        public List<SerializableActiveQuest> activeQuests = new List<SerializableActiveQuest>();

        // Relationships
        public Dictionary<string, int> npcAffinity = new Dictionary<string, int>();
        public List<string> romanceUnlockedNpcIds = new List<string>();

        // Eclipse System / Abyss
        public float syncRate;
        public List<string> scannedEnemyIds = new List<string>();
        public List<string> secretsLog = new List<string>();
        public float abyssEnergy;
        public Dictionary<string, float> abyssResistances = new Dictionary<string, float>(); // DamageType.ToString() -> value
        public List<string> unlockedMemoryFragments = new List<string>();
        public List<string> absorbedEnemyInstanceIds = new List<string>();

        // World
        public List<string> unlockedRegionIds = new List<string>();
        public List<string> unlockedFastTravelPointIds = new List<string>();
        public string currentRegionId;
        public UnityEngineVector3 lastPlayerPosition;

        // Settings
        public GameSettings settings = new GameSettings();
    }

    [Serializable]
    public class SerializableActiveQuest
    {
        public string questId;
        public List<string> objectiveIds = new List<string>();
        public List<int> objectiveProgress = new List<int>();
    }

    /// <summary>Plain Vector3 substitute so SaveData has zero UnityEngine type dependencies and stays trivially testable/serializable.</summary>
    [Serializable]
    public struct UnityEngineVector3
    {
        public float x, y, z;
        public UnityEngineVector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
    }

    [Serializable]
    public class GameSettings
    {
        public int graphicsQualityLevel = 2; // 0=Low,1=Medium,2=High,3=Ultra
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public float voiceVolume = 1f;
        public float cameraSensitivity = 1f;
        public float uiTextScale = 1f;
        public bool invertCameraY;
    }
}
