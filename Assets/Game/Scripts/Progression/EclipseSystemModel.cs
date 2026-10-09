using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Core;
using RebirthEclipse.Character;

namespace RebirthEclipse.Progression
{
    /// <summary>
    /// The player-facing "Eclipse System" HUD reads every field from here — nothing in this
    /// class is decorative. Sync Rate, scanned-enemy data, and secret-event log are all
    /// derived from real game state (quests completed, enemies scanned, absorptions performed),
    /// never incremented just to animate a number.
    /// </summary>
    public class EclipseSystemModel : MonoBehaviour
    {
        public static EclipseSystemModel Instance { get; private set; }

        [SerializeField] private CharacterStats playerStats;
        [SerializeField] private LevelingSystem leveling;
        [SerializeField] private ClassDefinition currentClass;

        private readonly HashSet<string> scannedEnemyIds = new HashSet<string>();
        private readonly Dictionary<string, EnemyScanResult> scanResults = new Dictionary<string, EnemyScanResult>();
        private readonly List<string> secretsLog = new List<string>();
        private readonly HashSet<string> storyFlags = new HashSet<string>();

        /// <summary>
        /// Sync Rate represents how fully the System has bonded with Arden's soul.
        /// It only rises from real milestones: class rank-ups, absorbed memory fragments,
        /// and specific story flags — never from grinding XP alone.
        /// </summary>
        public float SyncRate { get; private set; }

        private const float SyncPerClassStage = 0.05f;
        private const float SyncPerMemoryFragment = 0.02f;
        private const float SyncPerMajorStoryFlag = 0.03f;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnClassEvolutionStage += HandleClassEvolution;
            GameEvents.OnMemoryFragmentUnlocked += HandleMemoryFragment;
            GameEvents.OnStoryFlagSet += HandleStoryFlag;
        }

        private void OnDisable()
        {
            GameEvents.OnClassEvolutionStage -= HandleClassEvolution;
            GameEvents.OnMemoryFragmentUnlocked -= HandleMemoryFragment;
            GameEvents.OnStoryFlagSet -= HandleStoryFlag;
        }

        private void HandleClassEvolution(string classId, int stage) => IncreaseSync(SyncPerClassStage);
        private void HandleMemoryFragment(string fragmentId) => IncreaseSync(SyncPerMemoryFragment);

        private static readonly HashSet<string> MajorFlags = new HashSet<string>
        {
            "arc1_betrayal_witnessed", "arc3_first_awakening", "arc8_system_truth_revealed",
            "arc12_source_confronted"
        };

        private void HandleStoryFlag(string flagId)
        {
            storyFlags.Add(flagId);
            if (MajorFlags.Contains(flagId)) IncreaseSync(SyncPerMajorStoryFlag);
        }

        private void IncreaseSync(float amount)
        {
            SyncRate = Mathf.Clamp01(SyncRate + amount);
            GameEvents.RaiseSyncRateChanged(SyncRate);
        }

        public struct EnemyScanResult
        {
            public string enemyId;
            public string knownWeakness;      // e.g. "Curse", "Fire", "Backstab window during roar"
            public DamageType weakDamageType;
            public bool absorbable;
            public string lockedMemoryFragmentId; // empty if none
        }

        public void RegisterScan(string enemyId, EnemyScanResult result)
        {
            scannedEnemyIds.Add(enemyId);
            scanResults[enemyId] = result;
        }

        public bool HasScanned(string enemyId) => scannedEnemyIds.Contains(enemyId);

        public bool TryGetScanResult(string enemyId, out EnemyScanResult result) =>
            scanResults.TryGetValue(enemyId, out result);

        public void LogSecret(string secretDescriptionKey)
        {
            if (!secretsLog.Contains(secretDescriptionKey))
            {
                secretsLog.Add(secretDescriptionKey);
            }
        }

        public IReadOnlyList<string> SecretsLog => secretsLog;
        public IReadOnlyCollection<string> ScannedEnemies => scannedEnemyIds;

        /// <summary>Snapshot used by the HUD each frame/refresh — always built from live data.</summary>
        public EclipseSystemSnapshot BuildSnapshot()
        {
            return new EclipseSystemSnapshot
            {
                characterName = "Arden Vale",
                level = leveling != null ? leveling.CurrentLevel : 1,
                className = currentClass != null ? currentClass.displayName : "Unknown",
                rank = currentClass != null ? currentClass.startingRank.ToString() : "F",
                currentHealth = playerStats != null ? playerStats.CurrentHealth : 0,
                maxHealth = playerStats != null ? playerStats.Derived.maxHealth : 0,
                currentMana = playerStats != null ? playerStats.CurrentMana : 0,
                maxMana = playerStats != null ? playerStats.Derived.maxMana : 0,
                currentStamina = playerStats != null ? playerStats.CurrentStamina : 0,
                maxStamina = playerStats != null ? playerStats.Derived.maxStamina : 0,
                freeStatPoints = leveling != null ? leveling.UnspentStatPoints : 0,
                skillPoints = leveling != null ? leveling.UnspentSkillPoints : 0,
                syncRatePercent = Mathf.RoundToInt(SyncRate * 100f)
            };
        }

        public void LoadFromSave(float savedSync, IEnumerable<string> savedScanned, IEnumerable<string> savedSecrets, IEnumerable<string> savedFlags)
        {
            SyncRate = savedSync;
            scannedEnemyIds.Clear();
            foreach (var id in savedScanned) scannedEnemyIds.Add(id);
            secretsLog.Clear();
            secretsLog.AddRange(savedSecrets);
            storyFlags.Clear();
            foreach (var f in savedFlags) storyFlags.Add(f);
        }
    }

    public struct EclipseSystemSnapshot
    {
        public string characterName;
        public int level;
        public string className;
        public string rank;
        public float currentHealth, maxHealth;
        public float currentMana, maxMana;
        public float currentStamina, maxStamina;
        public int freeStatPoints;
        public int skillPoints;
        public int syncRatePercent;
    }
}
