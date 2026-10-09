using System;
using System.Collections.Generic;
using UnityEngine;
using RebirthEclipse.Character;

namespace RebirthEclipse.Combat
{
    public enum StatusEffectKind { Burn, Curse, Stun, Slow, Bleed, WeaknessExposed }

    [Serializable]
    public class StatusEffectInstance
    {
        public string effectId;
        public StatusEffectKind kind;
        public float remainingSeconds;
        public float tickDamagePerSecond;
        public float moveSpeedMultiplier = 1f;
        public bool blocksActions;
    }

    /// <summary>
    /// Runs damage-over-time / debuff ticks for one character. Attached alongside
    /// CharacterStats on any actor that can be afflicted (enemies, bosses, and the player).
    /// </summary>
    public class StatusEffectController : MonoBehaviour
    {
        private readonly List<StatusEffectInstance> activeEffects = new List<StatusEffectInstance>();
        private CharacterStats stats;

        public bool IsActionLocked { get; private set; }
        public float CurrentMoveSpeedMultiplier { get; private set; } = 1f;

        private void Awake() => stats = GetComponent<CharacterStats>();

        public void ApplyEffect(StatusEffectInstance effect)
        {
            var existing = activeEffects.Find(e => e.effectId == effect.effectId);
            if (existing != null)
            {
                existing.remainingSeconds = Mathf.Max(existing.remainingSeconds, effect.remainingSeconds);
            }
            else
            {
                activeEffects.Add(effect);
            }
        }

        private void Update()
        {
            if (activeEffects.Count == 0)
            {
                IsActionLocked = false;
                CurrentMoveSpeedMultiplier = 1f;
                return;
            }

            bool locked = false;
            float slowest = 1f;

            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var e = activeEffects[i];
                if (e.tickDamagePerSecond > 0f && !stats.IsDead)
                {
                    stats.ApplyDamage(e.tickDamagePerSecond * Time.deltaTime,
                        e.kind == StatusEffectKind.Curse ? Core.DamageType.Curse : Core.DamageType.Physical);
                }
                locked |= e.blocksActions;
                slowest = Mathf.Min(slowest, e.moveSpeedMultiplier);

                e.remainingSeconds -= Time.deltaTime;
                if (e.remainingSeconds <= 0f)
                {
                    activeEffects.RemoveAt(i);
                }
            }

            IsActionLocked = locked;
            CurrentMoveSpeedMultiplier = slowest;
        }

        public bool HasEffect(string effectId) => activeEffects.Exists(e => e.effectId == effectId);
        public void ClearAll() => activeEffects.Clear();
    }
}
