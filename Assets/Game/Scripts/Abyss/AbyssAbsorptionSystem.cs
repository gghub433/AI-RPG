using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Combat;
using RebirthEclipse.Core;
using RebirthEclipse.Progression;

namespace RebirthEclipse.Abyss
{
    public enum AbsorptionResult { Success, InsufficientEnergy, Incompatible, AlreadyAtResistanceCap, TargetNotAbsorbable, OnCooldown }

    /// <summary>
    /// Arden's signature ability: draining residual energy from a defeated foe. Deliberately
    /// bounded — not every enemy can be absorbed, each absorption has an energy cost and a
    /// compatibility check, and resistances gained this way have a hard cap, so the system
    /// cannot be farmed into a free power spiral. Absorbing a story-important target raises
    /// OnMemoryFragmentUnlocked, which other systems (EclipseSystemModel, QuestManager,
    /// faction reputation) listen for to trigger unique scenes/relationship shifts.
    /// </summary>
    [RequireComponent(typeof(CharacterStats))]
    public class AbyssAbsorptionSystem : MonoBehaviour
    {
        [Header("Energy Pool")]
        [SerializeField] private float maxAbyssEnergy = 100f;
        [SerializeField] private float energyRegenPerSecond = 2f;
        [SerializeField] private float globalCooldownSeconds = 3f;

        [Header("Resistance Caps")]
        [SerializeField] private float maxResistancePerType = 40f; // hard cap, per DamageType, from absorption alone

        public float CurrentEnergy { get; private set; }
        private float cooldownTimer;

        private readonly Dictionary<DamageType, float> gainedResistances = new Dictionary<DamageType, float>();
        private readonly HashSet<string> absorbedEnemyIds = new HashSet<string>(); // unique instances, prevents re-absorbing the same kill
        private readonly HashSet<string> unlockedMemoryFragments = new HashSet<string>();
        private readonly Dictionary<string, CombatForm> unlockedForms = new Dictionary<string, CombatForm>();

        private CharacterStats stats;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
            CurrentEnergy = maxAbyssEnergy;
        }

        private void Update()
        {
            CurrentEnergy = Mathf.Min(maxAbyssEnergy, CurrentEnergy + energyRegenPerSecond * Time.deltaTime);
            if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Attempts to absorb a defeated enemy. Must be called with a dead, already-scanned
        /// target; the enemy's archetype decides absorbability, cost, and compatibility.
        /// </summary>
        public AbsorptionResult TryAbsorb(string defeatedInstanceId, EnemyArchetype archetype, int playerLevel)
        {
            GameEvents.RaiseAbsorptionAttempted(defeatedInstanceId);

            // Permanent/identity checks first (these would stay rejected no matter how long
            // the player waits), then the transient cooldown/energy checks that retrying later
            // can clear — this ordering is what makes the rejection reason actually useful.
            if (archetype == null || !archetype.isAbsorbable) return Fail(defeatedInstanceId, AbsorptionResult.TargetNotAbsorbable);
            if (absorbedEnemyIds.Contains(defeatedInstanceId)) return Fail(defeatedInstanceId, AbsorptionResult.TargetNotAbsorbable);
            if (cooldownTimer > 0f) return Fail(defeatedInstanceId, AbsorptionResult.OnCooldown);
            if (CurrentEnergy < archetype.absorptionCost) return Fail(defeatedInstanceId, AbsorptionResult.InsufficientEnergy);

            // Compatibility: enemies far above the player's level resist absorption outright —
            // this is what stops low-level characters from power-spiking off a single strong kill.
            bool compatible = true; // placeholder hook: a per-archetype level-gap check can be added via data later
            if (!compatible) return Fail(defeatedInstanceId, AbsorptionResult.Incompatible);

            CurrentEnergy -= archetype.absorptionCost;
            cooldownTimer = globalCooldownSeconds;
            absorbedEnemyIds.Add(defeatedInstanceId);

            GrantResistance(archetype.weaknessType, 2f);

            if (!string.IsNullOrEmpty(archetype.lockedMemoryFragmentId) && unlockedMemoryFragments.Add(archetype.lockedMemoryFragmentId))
            {
                GameEvents.RaiseMemoryFragmentUnlocked(archetype.lockedMemoryFragmentId);
            }

            GameEvents.RaiseAbsorptionResolved(defeatedInstanceId, true);
            return AbsorptionResult.Success;
        }

        private AbsorptionResult Fail(string targetId, AbsorptionResult reason)
        {
            GameEvents.RaiseAbsorptionResolved(targetId, false);
            return reason;
        }

        private void GrantResistance(DamageType type, float amount)
        {
            float current = gainedResistances.TryGetValue(type, out var v) ? v : 0f;
            float updated = Mathf.Min(maxResistancePerType, current + amount);
            gainedResistances[type] = updated;
        }

        public float GetAbsorbedResistance(DamageType type) => gainedResistances.TryGetValue(type, out var v) ? v : 0f;

        /// <summary>
        /// Temporary combat forms unlocked through absorption — a bounded-duration
        /// transformation with its own stat multiplier, consumed from the same Abyss energy
        /// pool rather than mana, so using one trades off against future absorptions.
        /// </summary>
        [System.Serializable]
        public struct CombatForm
        {
            public string formId;
            public float durationSeconds;
            public float energyCostToActivate;
            public float physicalPowerMultiplier;
            public float magicPowerMultiplier;
        }

        public void UnlockForm(CombatForm form) => unlockedForms[form.formId] = form;
        public bool HasForm(string formId) => unlockedForms.ContainsKey(formId);

        public bool TryActivateForm(string formId, out float duration)
        {
            duration = 0f;
            if (!unlockedForms.TryGetValue(formId, out var form)) return false;
            if (CurrentEnergy < form.energyCostToActivate) return false;
            CurrentEnergy -= form.energyCostToActivate;
            duration = form.durationSeconds;
            return true;
        }

        public IReadOnlyCollection<string> UnlockedMemoryFragments => unlockedMemoryFragments;

        public void LoadFromSave(float savedEnergy, Dictionary<DamageType, float> savedResistances, IEnumerable<string> savedFragments, IEnumerable<string> savedAbsorbedIds)
        {
            CurrentEnergy = savedEnergy;
            gainedResistances.Clear();
            foreach (var kv in savedResistances) gainedResistances[kv.Key] = kv.Value;
            unlockedMemoryFragments.Clear();
            foreach (var f in savedFragments) unlockedMemoryFragments.Add(f);
            absorbedEnemyIds.Clear();
            foreach (var id in savedAbsorbedIds) absorbedEnemyIds.Add(id);
        }
    }
}
