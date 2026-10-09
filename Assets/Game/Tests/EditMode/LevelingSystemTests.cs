using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Progression;

namespace RebirthEclipse.Tests
{
    public class LevelingSystemTests
    {
        private GameObject go;
        private LevelingSystem leveling;
        private CharacterStats stats;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("TestCharacter");
            stats = go.AddComponent<CharacterStats>();
            leveling = go.AddComponent<LevelingSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
        }

        [Test]
        public void XpRequiredForLevel_IsMonotonicallyIncreasing()
        {
            int prev = 0;
            for (int lvl = 2; lvl <= 20; lvl++)
            {
                int required = leveling.XpRequiredForLevel(lvl);
                Assert.Greater(required, prev, $"Level {lvl} should require more XP than level {lvl - 1}");
                prev = required;
            }
        }

        [Test]
        public void GrantExperience_BelowThreshold_DoesNotLevelUp()
        {
            int needed = leveling.XpRequiredForNextLevel();
            leveling.GrantExperience(needed - 1);
            Assert.AreEqual(1, leveling.CurrentLevel);
        }

        [Test]
        public void GrantExperience_AtThreshold_LevelsUpExactlyOnce()
        {
            int needed = leveling.XpRequiredForNextLevel();
            leveling.GrantExperience(needed);
            Assert.AreEqual(2, leveling.CurrentLevel);
        }

        [Test]
        public void GrantExperience_LargeAmount_CanLevelUpMultipleTimes()
        {
            int needed = leveling.XpRequiredForLevel(5);
            leveling.GrantExperience(needed);
            Assert.AreEqual(5, leveling.CurrentLevel);
        }

        [Test]
        public void LevelUp_GrantsStatAndSkillPoints()
        {
            int needed = leveling.XpRequiredForNextLevel();
            leveling.GrantExperience(needed);
            Assert.Greater(leveling.UnspentStatPoints, 0);
            Assert.Greater(leveling.UnspentSkillPoints, 0);
        }

        [Test]
        public void TrySpendStatPoint_WithoutPoints_Fails()
        {
            bool result = leveling.TrySpendMultipleStatPoints("strength", 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySpendStatPoint_WithPoints_Succeeds_AndDecrementsPool()
        {
            leveling.GrantExperience(leveling.XpRequiredForNextLevel());
            int before = leveling.UnspentStatPoints;
            bool result = leveling.TrySpendStatPoint("strength");
            Assert.IsTrue(result);
            Assert.AreEqual(before - 1, leveling.UnspentStatPoints);
        }

        [Test]
        public void CharacterLevel_MirrorsLevelingSystemLevel_AfterLevelUp()
        {
            leveling.GrantExperience(leveling.XpRequiredForLevel(3));
            Assert.AreEqual(leveling.CurrentLevel, stats.Level);
        }
    }
}
