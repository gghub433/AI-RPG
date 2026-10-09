using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Abyss;
using RebirthEclipse.Character;
using RebirthEclipse.Combat;

namespace RebirthEclipse.Tests
{
    public class AbyssAbsorptionTests
    {
        private GameObject go;
        private AbyssAbsorptionSystem abyss;
        private EnemyArchetype absorbableArchetype;
        private EnemyArchetype nonAbsorbableArchetype;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("TestAbyssOwner");
            go.AddComponent<CharacterStats>();
            abyss = go.AddComponent<AbyssAbsorptionSystem>();

            absorbableArchetype = ScriptableObject.CreateInstance<EnemyArchetype>();
            absorbableArchetype.enemyId = "shade_wolf";
            absorbableArchetype.isAbsorbable = true;
            absorbableArchetype.absorptionCost = 10f;

            nonAbsorbableArchetype = ScriptableObject.CreateInstance<EnemyArchetype>();
            nonAbsorbableArchetype.enemyId = "boss_unabsorbable";
            nonAbsorbableArchetype.isAbsorbable = false;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(absorbableArchetype);
            Object.DestroyImmediate(nonAbsorbableArchetype);
        }

        [Test]
        public void Absorb_NonAbsorbableArchetype_IsRejected()
        {
            var result = abyss.TryAbsorb("instance-1", nonAbsorbableArchetype, 1);
            Assert.AreEqual(AbsorptionResult.TargetNotAbsorbable, result);
        }

        [Test]
        public void Absorb_SameDefeatedInstanceTwice_SecondAttemptRejected()
        {
            var first = abyss.TryAbsorb("instance-1", absorbableArchetype, 1);
            var second = abyss.TryAbsorb("instance-1", absorbableArchetype, 1);

            Assert.AreEqual(AbsorptionResult.Success, first);
            Assert.AreEqual(AbsorptionResult.TargetNotAbsorbable, second,
                "Absorbing the exact same defeated enemy instance twice must be blocked.");
        }

        [Test]
        public void Absorb_ConsumesEnergy_ByArchetypeCost()
        {
            float before = abyss.CurrentEnergy;
            abyss.TryAbsorb("instance-1", absorbableArchetype, 1);
            Assert.AreEqual(before - absorbableArchetype.absorptionCost, abyss.CurrentEnergy);
        }

        [Test]
        public void Absorb_WithInsufficientEnergy_Fails_AndDoesNotConsumeAnyEnergy()
        {
            absorbableArchetype.absorptionCost = 99999f;
            float before = abyss.CurrentEnergy;
            var result = abyss.TryAbsorb("instance-1", absorbableArchetype, 1);
            Assert.AreEqual(AbsorptionResult.InsufficientEnergy, result);
            Assert.AreEqual(before, abyss.CurrentEnergy);
        }

        [Test]
        public void ResistanceGain_IsCappedByMaxResistancePerType()
        {
            // Absorb many low-cost targets back to back (bypassing cooldown via distinct test
            // instances is irrelevant here — we only check the resistance accumulator's cap).
            var field = typeof(AbyssAbsorptionSystem).GetField("maxResistancePerType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            float cap = (float)field.GetValue(abyss);

            for (int i = 0; i < 50; i++)
            {
                abyss.GetType()
                    .GetMethod("GrantResistance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(abyss, new object[] { Core.DamageType.Physical, 5f });
            }

            Assert.LessOrEqual(abyss.GetAbsorbedResistance(Core.DamageType.Physical), cap);
        }
    }
}
