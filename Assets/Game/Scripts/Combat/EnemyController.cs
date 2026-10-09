using UnityEngine;
using UnityEngine.AI;
using RebirthEclipse.Character;
using RebirthEclipse.Core;

namespace RebirthEclipse.Combat
{
    public enum EnemyBehaviorState { Idle, Patrol, Chase, Attack, Staggered, Dead }

    /// <summary>
    /// Base AI for standard enemies. Each enemy TYPE gets its own small subclass (or, for
    /// simple cases, just a different EnemyArchetype data asset) rather than everyone sharing
    /// one monolithic brain — satisfies "different combat logic per enemy type" from the
    /// combat design spec. Boss-tier enemies use BossController instead, which extends this
    /// with phases.
    /// </summary>
    [RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected EnemyArchetype archetype;

        [Header("Detection")]
        [SerializeField] protected float detectionRange = 10f;
        [SerializeField] protected float attackRange = 2.2f;
        [SerializeField] protected float loseTargetRange = 18f;

        protected CharacterStats stats;
        protected StatusEffectController statusEffects;
        protected NavMeshAgent agent;
        protected CharacterStats player;
        protected EnemyBehaviorState state = EnemyBehaviorState.Idle;
        protected float attackCooldownTimer;

        protected virtual void Awake()
        {
            stats = GetComponent<CharacterStats>();
            statusEffects = GetComponent<StatusEffectController>();
            agent = GetComponent<NavMeshAgent>();
        }

        protected virtual void OnEnable()
        {
            GameEvents.OnCharacterDied += HandleAnyDeath;
        }

        protected virtual void OnDisable()
        {
            GameEvents.OnCharacterDied -= HandleAnyDeath;
        }

        public void SetPlayerReference(CharacterStats playerStats) => player = playerStats;

        private void HandleAnyDeath(CharacterStats who)
        {
            if (who == stats)
            {
                state = EnemyBehaviorState.Dead;
                agent.isStopped = true;
            }
        }

        protected virtual void Update()
        {
            if (state == EnemyBehaviorState.Dead || player == null) return;
            if (statusEffects != null && statusEffects.IsActionLocked)
            {
                agent.isStopped = true;
                return;
            }
            agent.isStopped = false;

            float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

            switch (state)
            {
                case EnemyBehaviorState.Idle:
                case EnemyBehaviorState.Patrol:
                    if (distToPlayer <= detectionRange) state = EnemyBehaviorState.Chase;
                    break;

                case EnemyBehaviorState.Chase:
                    if (distToPlayer > loseTargetRange) { state = EnemyBehaviorState.Idle; break; }
                    if (distToPlayer <= attackRange) { state = EnemyBehaviorState.Attack; break; }
                    agent.SetDestination(player.transform.position);
                    break;

                case EnemyBehaviorState.Attack:
                    if (distToPlayer > attackRange * 1.2f) { state = EnemyBehaviorState.Chase; break; }
                    attackCooldownTimer -= Time.deltaTime;
                    if (attackCooldownTimer <= 0f)
                    {
                        PerformAttack();
                        attackCooldownTimer = archetype != null ? archetype.attackIntervalSeconds : 1.5f;
                    }
                    break;
            }
        }

        protected virtual void PerformAttack()
        {
            if (player == null || player.IsDead) return;
            var combat = player.GetComponent<CombatController>();
            if (combat != null && combat.TryDefendIncomingHit(out bool wasParry))
            {
                if (wasParry)
                {
                    // Successful parry staggers the attacker.
                    state = EnemyBehaviorState.Staggered;
                    Invoke(nameof(RecoverFromStagger), 1.2f);
                }
                return;
            }

            float basePower = archetype != null ? archetype.baseAttackPower : 8f;
            var result = DamageCalculator.Compute(stats, player, basePower, false, DamageType.Physical, Random.value);
            player.ApplyDamage(result.finalDamage, DamageType.Physical);
        }

        protected void RecoverFromStagger()
        {
            if (state == EnemyBehaviorState.Staggered) state = EnemyBehaviorState.Chase;
        }
    }

    /// <summary>Data describing one enemy type — stats baseline, attack timing, loot table key.</summary>
    [CreateAssetMenu(fileName = "NewEnemyArchetype", menuName = "RebirthEclipse/Enemy Archetype")]
    public class EnemyArchetype : ScriptableObject
    {
        public string enemyId;
        public string displayName;
        public float baseAttackPower = 8f;
        public float attackIntervalSeconds = 1.5f;
        public DamageType weaknessType = DamageType.Physical;
        public bool isAbsorbable = true;
        public float absorptionCost = 10f; // Abyss energy cost to absorb this enemy
        public string lootTableId;
        public string lockedMemoryFragmentId;
    }
}
