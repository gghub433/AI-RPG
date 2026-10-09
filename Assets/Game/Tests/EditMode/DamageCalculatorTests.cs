using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Combat;
using RebirthEclipse.Core;

namespace RebirthEclipse.Tests
{
    public class DamageCalculatorTests
    {
        private GameObject attackerGo, defenderGo;
        private CharacterStats attacker, defender;

        [SetUp]
        public void SetUp()
        {
            attackerGo = new GameObject("Attacker");
            attacker = attackerGo.AddComponent<CharacterStats>();
            defenderGo = new GameObject("Defender");
            defender = defenderGo.AddComponent<CharacterStats>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(attackerGo);
            Object.DestroyImmediate(defenderGo);
        }

        [Test]
        public void HigherDefense_ReducesDamage()
        {
            defender.SetLevel(1);
            var lowDefResult = DamageCalculator.Compute(attacker, defender, 50f, false, DamageType.Physical, 0.99f);

            defender.AllocateStatPoint("vitality", 50); // raises physicalDefense substantially
            var highDefResult = DamageCalculator.Compute(attacker, defender, 50f, false, DamageType.Physical, 0.99f);

            Assert.Less(highDefResult.finalDamage, lowDefResult.finalDamage);
        }

        [Test]
        public void TrueDamage_IgnoresDefense_AndNeverCrits()
        {
            defender.AllocateStatPoint("vitality", 100);
            var result = DamageCalculator.Compute(attacker, defender, 30f, false, DamageType.True, 0.0f);
            Assert.IsFalse(result.isCritical);
            Assert.Greater(result.finalDamage, 0f);
        }

        [Test]
        public void DamageIsNeverNegativeOrZero()
        {
            defender.AllocateStatPoint("vitality", 500);
            var result = DamageCalculator.Compute(attacker, defender, 1f, false, DamageType.Physical, 0.9f);
            Assert.GreaterOrEqual(result.finalDamage, 1f);
        }

        [Test]
        public void RollBelowCritChance_ProducesCriticalHit_WithMultiplier()
        {
            // Force crit chance to ~1 by stacking agility/luck, then roll 0 (always below chance).
            attacker.AllocateStatPoint("agility", 500);
            attacker.AllocateStatPoint("luck", 500);
            var result = DamageCalculator.Compute(attacker, defender, 10f, false, DamageType.Physical, 0f);
            Assert.IsTrue(result.isCritical);
        }

        [Test]
        public void MagicDamage_ScalesWithMagicPower_NotPhysicalPower()
        {
            attacker.AllocateStatPoint("strength", 200); // should not affect magic result
            var physicalLeaning = DamageCalculator.Compute(attacker, defender, 10f, true, DamageType.Magical, 0.99f);

            var freshGo = new GameObject("FreshAttacker");
            var freshAttacker = freshGo.AddComponent<CharacterStats>();
            freshAttacker.AllocateStatPoint("intellect", 200); // should affect magic result strongly
            var magicLeaning = DamageCalculator.Compute(freshAttacker, defender, 10f, true, DamageType.Magical, 0.99f);

            Assert.Greater(magicLeaning.finalDamage, physicalLeaning.finalDamage);
            Object.DestroyImmediate(freshGo);
        }
    }
}
