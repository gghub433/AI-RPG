using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Character;
using RebirthEclipse.Core;

namespace RebirthEclipse.Tests
{
    public class HeroStageTests
    {
        private GameObject go;
        private HeroStageController hero;
        private HashSet<string> flags;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject("Arden");
            go.AddComponent<CharacterStats>();
            hero = go.AddComponent<HeroStageController>();

            var stages = new[]
            {
                new HeroStage { stageId = "reborn", displayName = "Reborn", powerMultiplier = 1f },
                new HeroStage { stageId = "abyss", displayName = "Abyss Awakening", requiredStoryFlag = "f_abyss", requiredSyncRate = 0.2f, powerMultiplier = 1.3f },
                new HeroStage { stageId = "sovereign", displayName = "Eclipse Sovereign", requiredStoryFlag = "f_sovereign", requiredSyncRate = 0.5f, powerMultiplier = 1.8f },
            };
            typeof(HeroStageController)
                .GetField("stages", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(hero, stages);

            flags = new HashSet<string>();
            hero.StoryFlagCheck = f => flags.Contains(f);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(go);

        [Test]
        public void StartsAtStageZero()
        {
            Assert.AreEqual(0, hero.CurrentStageIndex);
            Assert.AreEqual(1f, hero.CurrentPowerMultiplier);
        }

        [Test]
        public void DoesNotAdvance_WhenOnlySyncRateMet_ButFlagMissing()
        {
            GameEvents.RaiseSyncRateChanged(0.9f); // plenty of sync, but no story flag
            Assert.AreEqual(0, hero.CurrentStageIndex);
        }

        [Test]
        public void DoesNotAdvance_WhenOnlyFlagMet_ButSyncTooLow()
        {
            flags.Add("f_abyss");
            GameEvents.RaiseStoryFlagSet("f_abyss");
            GameEvents.RaiseSyncRateChanged(0.1f); // below the 0.2 threshold
            Assert.AreEqual(0, hero.CurrentStageIndex);
        }

        [Test]
        public void Advances_WhenBothFlagAndSyncMet()
        {
            flags.Add("f_abyss");
            GameEvents.RaiseStoryFlagSet("f_abyss");
            GameEvents.RaiseSyncRateChanged(0.25f);
            Assert.AreEqual(1, hero.CurrentStageIndex);
            Assert.AreEqual(1.3f, hero.CurrentPowerMultiplier);
        }

        [Test]
        public void DoesNotSkipLockedIntermediateStage()
        {
            // Meet stage 2's requirements but NOT stage 1's flag — must stay at 0,
            // because stages gate sequentially (can't skip Abyss Awakening to reach Sovereign).
            flags.Add("f_sovereign");
            GameEvents.RaiseStoryFlagSet("f_sovereign");
            GameEvents.RaiseSyncRateChanged(0.9f);
            Assert.AreEqual(0, hero.CurrentStageIndex);
        }

        [Test]
        public void Advances_SequentiallyToHighestEligibleStage()
        {
            flags.Add("f_abyss");
            flags.Add("f_sovereign");
            GameEvents.RaiseStoryFlagSet("f_sovereign");
            GameEvents.RaiseSyncRateChanged(0.6f); // meets both stage 1 (0.2) and stage 2 (0.5)
            Assert.AreEqual(2, hero.CurrentStageIndex);
        }

        [Test]
        public void StageIsPermanent_DoesNotRegressIfSyncRateDrops()
        {
            flags.Add("f_abyss");
            GameEvents.RaiseStoryFlagSet("f_abyss");
            GameEvents.RaiseSyncRateChanged(0.5f);
            Assert.AreEqual(1, hero.CurrentStageIndex);

            GameEvents.RaiseSyncRateChanged(0.0f); // sync collapses
            Assert.AreEqual(1, hero.CurrentStageIndex, "Hero stages are permanent once achieved.");
        }
    }
}
