using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RebirthEclipse.Character;
using RebirthEclipse.Core;
using RebirthEclipse.Progression;

namespace RebirthEclipse.Combat
{
    /// <summary>
    /// Player combat input -> action resolution. Handles light/heavy combo chains, dodge,
    /// block, skill activation (reading from the equipped SkillTreeDefinition), and target
    /// lock. Enemies use their own EnemyController/BossController, which call into the same
    /// DamageCalculator so numbers stay consistent on both sides of a fight.
    /// </summary>
    [RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(StatusEffectController))]
    public class CombatController : MonoBehaviour
    {
        [Header("Combo")]
        [SerializeField] private float comboWindowSeconds = 0.8f;
        [SerializeField] private float[] lightComboMultipliers = { 1.0f, 1.1f, 1.3f };
        [SerializeField] private float heavyAttackMultiplier = 2.0f;
        [SerializeField] private float heavyStaminaCost = 25f;
        [SerializeField] private float lightStaminaCost = 10f;

        [Header("Dodge / Block")]
        [SerializeField] private float dodgeStaminaCost = 15f;
        [SerializeField] private float dodgeInvulnSeconds = 0.3f;
        [SerializeField] private float parryWindowSeconds = 0.2f;

        [Header("Targeting")]
        [SerializeField] private float targetLockRange = 12f;
        [SerializeField] private LayerMask enemyLayerMask;

        private CharacterStats stats;
        private StatusEffectController statusEffects;
        private int comboIndex;
        private float comboTimer;
        private float dodgeInvulnTimer;
        private bool isBlocking;
        private float blockStartTime;
        private readonly Dictionary<string, float> skillCooldowns = new Dictionary<string, float>();

        public CharacterStats CurrentTarget { get; private set; }
        public bool IsInvulnerable => dodgeInvulnTimer > 0f;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
            statusEffects = GetComponent<StatusEffectController>();
        }

        private void Update()
        {
            if (comboTimer > 0f)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f) comboIndex = 0;
            }
            if (dodgeInvulnTimer > 0f) dodgeInvulnTimer -= Time.deltaTime;

            var keys = new List<string>(skillCooldowns.Keys);
            foreach (var key in keys)
            {
                skillCooldowns[key] = Mathf.Max(0f, skillCooldowns[key] - Time.deltaTime);
            }
        }

        // ---- Input System callbacks ----

        public void OnLightAttack(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || IsActionBlocked()) return;
            PerformLightAttack();
        }

        public void OnHeavyAttack(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || IsActionBlocked()) return;
            PerformHeavyAttack();
        }

        public void OnDodge(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed || stats.IsDead) return;
            PerformDodge();
        }

        public void OnBlockStart(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) { isBlocking = true; blockStartTime = Time.time; }
        }

        public void OnBlockEnd(InputAction.CallbackContext ctx)
        {
            if (ctx.canceled) isBlocking = false;
        }

        public void OnTargetLock(InputAction.CallbackContext ctx)
        {
            if (ctx.performed) AcquireNearestTarget();
        }

        // ---- Actions ----

        private bool IsActionBlocked() => stats.IsDead || (statusEffects != null && statusEffects.IsActionLocked);

        private void PerformLightAttack()
        {
            if (!stats.TrySpendStamina(lightStaminaCost)) return;
            comboTimer = comboWindowSeconds;
            float mult = lightComboMultipliers[Mathf.Min(comboIndex, lightComboMultipliers.Length - 1)];
            comboIndex = (comboIndex + 1) % lightComboMultipliers.Length;

            if (CurrentTarget != null && !CurrentTarget.IsDead)
            {
                var result = DamageCalculator.Compute(stats, CurrentTarget, 10f * mult, false, DamageType.Physical, Random.value);
                CurrentTarget.ApplyDamage(result.finalDamage, DamageType.Physical);
            }
        }

        private void PerformHeavyAttack()
        {
            if (!stats.TrySpendStamina(heavyStaminaCost)) return;
            comboIndex = 0;
            comboTimer = 0f;

            if (CurrentTarget != null && !CurrentTarget.IsDead)
            {
                var result = DamageCalculator.Compute(stats, CurrentTarget, 10f * heavyAttackMultiplier, false, DamageType.Physical, Random.value);
                CurrentTarget.ApplyDamage(result.finalDamage, DamageType.Physical);
            }
        }

        private void PerformDodge()
        {
            if (!stats.TrySpendStamina(dodgeStaminaCost)) return;
            dodgeInvulnTimer = dodgeInvulnSeconds;
        }

        /// <summary>
        /// Call from an enemy's hit-landed path before applying damage to the player.
        /// Returns true if the hit should be fully negated (dodge i-frames) or mitigated
        /// to zero via a successful parry window.
        /// </summary>
        public bool TryDefendIncomingHit(out bool wasParry)
        {
            wasParry = false;
            if (dodgeInvulnTimer > 0f) return true;
            if (isBlocking)
            {
                if (Time.time - blockStartTime <= parryWindowSeconds)
                {
                    wasParry = true;
                    return true;
                }
                return true; // blocked, damage reduced elsewhere by defense stat — not fully negated
            }
            return false;
        }

        public bool TryActivateSkill(SkillNodeDefinition skill)
        {
            if (skill == null || IsActionBlocked()) return false;
            if (skillCooldowns.TryGetValue(skill.skillId, out float remaining) && remaining > 0f) return false;
            if (!stats.TrySpendMana(skill.manaCost)) return false;
            if (!stats.TrySpendStamina(skill.staminaCost)) return false;

            float cooldown = skill.cooldownSeconds * (1f - stats.Derived.cooldownReduction);
            skillCooldowns[skill.skillId] = cooldown;

            if (CurrentTarget != null && !CurrentTarget.IsDead)
            {
                var result = DamageCalculator.Compute(stats, CurrentTarget, skill.basePower, skill.scalesWithMagic, skill.damageType, Random.value);
                CurrentTarget.ApplyDamage(result.finalDamage, skill.damageType);

                if (!string.IsNullOrEmpty(skill.statusEffectId) && Random.value < skill.statusEffectChance)
                {
                    var effectController = CurrentTarget.GetComponent<StatusEffectController>();
                    effectController?.ApplyEffect(new StatusEffectInstance
                    {
                        effectId = skill.statusEffectId,
                        kind = StatusEffectKind.Curse,
                        remainingSeconds = 3f,
                        tickDamagePerSecond = skill.basePower * 0.1f
                    });
                }
            }
            return true;
        }

        private void AcquireNearestTarget()
        {
            var hits = Physics.OverlapSphere(transform.position, targetLockRange, enemyLayerMask);
            CharacterStats closest = null;
            float closestDist = float.MaxValue;
            foreach (var hit in hits)
            {
                var cs = hit.GetComponent<CharacterStats>();
                if (cs == null || cs.IsDead) continue;
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = cs;
                }
            }
            CurrentTarget = closest;
        }
    }
}
