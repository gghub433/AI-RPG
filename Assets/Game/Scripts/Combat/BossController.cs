using System;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Core;

namespace RebirthEclipse.Combat
{
    [Serializable]
    public class BossPhase
    {
        public string phaseName;
        [Range(0f, 1f)] public float healthThreshold; // phase activates when HP% drops to/below this
        public float attackPowerMultiplier = 1f;
        public float attackIntervalSeconds = 1.5f;
        public string[] uniqueAttackIds;      // identifiers the Animator/attack-selection logic reads
        public string telegraphAnimationTrigger; // clear tell before the phase's signature attack
        public string transitionCutsceneId;   // empty = no cutscene, just a visual/audio sting
        public string phaseStartDialogueId;
    }

    /// <summary>
    /// Multi-phase boss brain. Extends the standard enemy loop with HP-gated phase
    /// transitions, each with its own attack pattern, telegraph, and optional story beat —
    /// per the design requirement that bosses are not "the same enemy, bigger numbers."
    /// </summary>
    public class BossController : EnemyController
    {
        [Header("Boss Phases")]
        [SerializeField] private BossPhase[] phases;
        [SerializeField] private string bossId;

        private int currentPhaseIndex = -1;

        protected override void Update()
        {
            base.Update();
            if (state == EnemyBehaviorState.Dead) return;

            float hpFraction = stats.Derived.maxHealth > 0f ? stats.CurrentHealth / stats.Derived.maxHealth : 0f;
            for (int i = phases.Length - 1; i >= 0; i--)
            {
                if (hpFraction <= phases[i].healthThreshold && i > currentPhaseIndex)
                {
                    EnterPhase(i);
                    break;
                }
            }
        }

        private void EnterPhase(int index)
        {
            currentPhaseIndex = index;
            var phase = phases[index];
            attackCooldownTimer = phase.attackIntervalSeconds;
            GameEvents.RaiseBossPhaseChanged($"{bossId}:{index}");

            if (!string.IsNullOrEmpty(phase.telegraphAnimationTrigger))
            {
                var animator = GetComponentInChildren<Animator>();
                animator?.SetTrigger(phase.telegraphAnimationTrigger);
            }
            // Cutscene/dialogue triggering is wired by a scene-level BossEncounterDirector,
            // which listens for OnBossPhaseChanged and pulls transitionCutsceneId /
            // phaseStartDialogueId off this phase definition.
        }

        protected override void PerformAttack()
        {
            if (player == null || player.IsDead) return;
            var combat = player.GetComponent<CombatController>();
            if (combat != null && combat.TryDefendIncomingHit(out bool wasParry))
            {
                if (wasParry)
                {
                    state = EnemyBehaviorState.Staggered;
                    Invoke(nameof(RecoverFromStagger), 1.5f);
                }
                return;
            }

            float basePower = archetype != null ? archetype.baseAttackPower : 20f;
            float multiplier = currentPhaseIndex >= 0 ? phases[currentPhaseIndex].attackPowerMultiplier : 1f;
            var result = DamageCalculator.Compute(stats, player, basePower * multiplier, false, DamageType.Physical, UnityEngine.Random.value);
            player.ApplyDamage(result.finalDamage, DamageType.Physical);
        }

        public BossPhase CurrentPhase => currentPhaseIndex >= 0 && currentPhaseIndex < phases.Length ? phases[currentPhaseIndex] : null;
    }
}
