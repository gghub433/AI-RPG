using System;
using UnityEngine;
using RebirthEclipse.Core;

namespace RebirthEclipse.Character
{
    /// <summary>
    /// Arden's four signature evolution stages. Unlike the per-class evolution in
    /// Progression/ClassDefinition (which changes the skill kit), hero stages are the
    /// narrative/visual power tiers tied to the main story and Eclipse Sync Rate — Reborn,
    /// Abyss Awakening, Eclipse Sovereign, Absolute Eclipse. Each stage swaps the active
    /// armor/weapon visual set and applies a flat combat multiplier, and advancement is gated
    /// on BOTH a story flag and a Sync Rate threshold so it can never be reached by grinding
    /// alone.
    /// </summary>
    [Serializable]
    public class HeroStage
    {
        public string stageId;
        public string displayName;
        [TextArea(2, 4)] public string description;
        public string requiredStoryFlag;    // empty = no story gate
        [Range(0f, 1f)] public float requiredSyncRate;
        public float powerMultiplier = 1f;  // applied to physical/magic power
        public string armorSetId;            // visual set key (e.g. "eclipse_sovereign")
        public string weaponId;              // signature weapon for the stage
        public string auraVfxId;
    }

    [RequireComponent(typeof(CharacterStats))]
    public class HeroStageController : MonoBehaviour
    {
        [SerializeField] private HeroStage[] stages = new HeroStage[0];
        [SerializeField] private CharacterStats stats;

        public int CurrentStageIndex { get; private set; } = 0;
        public HeroStage CurrentStage => (stages != null && CurrentStageIndex >= 0 && CurrentStageIndex < stages.Length)
            ? stages[CurrentStageIndex] : null;

        private float lastSyncRate;

        private void Awake()
        {
            if (stats == null) stats = GetComponent<CharacterStats>();
        }

        private void OnEnable()
        {
            GameEvents.OnStoryFlagSet += HandleStoryFlag;
            GameEvents.OnSyncRateChanged += HandleSyncRate;
        }

        private void OnDisable()
        {
            GameEvents.OnStoryFlagSet -= HandleStoryFlag;
            GameEvents.OnSyncRateChanged -= HandleSyncRate;
        }

        private void HandleStoryFlag(string _) => ReevaluateStage();
        private void HandleSyncRate(float value) { lastSyncRate = value; ReevaluateStage(); }

        /// <summary>
        /// Advances to the highest stage whose requirements are currently met. Only ever moves
        /// forward — hero stages are permanent once achieved. Needs a flag-checker delegate
        /// because QuestManager owns the story flags; wired by the scene bootstrap.
        /// </summary>
        public Func<string, bool> StoryFlagCheck = _ => false;

        public void ReevaluateStage()
        {
            if (stages == null || stages.Length == 0) return;
            int best = CurrentStageIndex;
            for (int i = CurrentStageIndex + 1; i < stages.Length; i++)
            {
                var s = stages[i];
                bool flagOk = string.IsNullOrEmpty(s.requiredStoryFlag) || StoryFlagCheck(s.requiredStoryFlag);
                bool syncOk = lastSyncRate >= s.requiredSyncRate;
                if (flagOk && syncOk) best = i;
                else break; // stages are ordered; stop at the first unmet gate
            }
            if (best != CurrentStageIndex)
            {
                SetStage(best);
            }
        }

        private void SetStage(int index)
        {
            CurrentStageIndex = index;
            GameEvents.RaiseHeroStageChanged(index);
            // Visual swap (armorSetId/weaponId/auraVfxId) is handled by a presentation-layer
            // listener on OnHeroStageChanged; this controller only owns the gameplay state.
        }

        public float CurrentPowerMultiplier => CurrentStage != null ? CurrentStage.powerMultiplier : 1f;

        public void LoadFromSave(int savedStageIndex)
        {
            CurrentStageIndex = Mathf.Clamp(savedStageIndex, 0, Mathf.Max(0, (stages?.Length ?? 1) - 1));
        }
    }
}
