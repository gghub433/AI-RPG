using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Quests;
using RebirthEclipse.Progression;
using RebirthEclipse.Character;

namespace RebirthEclipse.Tests
{
    public class QuestManagerTests
    {
        private GameObject go;
        private QuestManager questManager;
        private QuestDatabase database;
        private QuestDefinition killQuest;
        private LevelingSystem leveling;
        private GameObject playerGo;

        [SetUp]
        public void SetUp()
        {
            killQuest = ScriptableObject.CreateInstance<QuestDefinition>();
            killQuest.questId = "q_kill_wolves";
            killQuest.minimumLevel = 1;
            killQuest.objectives = new[]
            {
                new QuestObjective { objectiveId = "obj_kill_wolf", type = ObjectiveType.KillTarget, targetId = "wolf", requiredCount = 3 }
            };
            killQuest.reward = new QuestReward { experience = 50 };

            database = ScriptableObject.CreateInstance<QuestDatabase>();
            database.allQuests = new[] { killQuest };

            playerGo = new GameObject("Player");
            var stats = playerGo.AddComponent<CharacterStats>();
            leveling = playerGo.AddComponent<LevelingSystem>();

            go = new GameObject("QuestManager");
            questManager = go.AddComponent<QuestManager>();
            SetPrivate(questManager, "questDatabase", database);
            SetPrivate(questManager, "playerLeveling", leveling);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(playerGo);
            Object.DestroyImmediate(killQuest);
            Object.DestroyImmediate(database);
        }

        private static void SetPrivate(object target, string field, object value)
        {
            target.GetType().GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(target, value);
        }

        [Test]
        public void StartQuest_BelowMinimumLevel_Fails()
        {
            killQuest.minimumLevel = 10;
            bool started = questManager.StartQuest("q_kill_wolves");
            Assert.IsFalse(started);
        }

        [Test]
        public void StartQuest_Valid_Succeeds_AndCreatesActiveState()
        {
            bool started = questManager.StartQuest("q_kill_wolves");
            Assert.IsTrue(started);
            Assert.IsTrue(questManager.ActiveQuests.ContainsKey("q_kill_wolves"));
        }

        [Test]
        public void Quest_DoesNotComplete_UntilAllRequiredKillsRegistered()
        {
            questManager.StartQuest("q_kill_wolves");

            var wolf1 = new GameObject("Wolf1");
            var wolfStats1 = wolf1.AddComponent<CharacterStats>();
            wolfStats1.characterId = "wolf";
            wolfStats1.isPlayer = false;

            // Simulate two kills — short of the required 3.
            Core.GameEvents.RaiseCharacterDied(wolfStats1);
            Core.GameEvents.RaiseCharacterDied(wolfStats1);

            Assert.IsTrue(questManager.ActiveQuests.ContainsKey("q_kill_wolves"), "Quest must not complete before required count is met.");
            Assert.IsFalse(questManager.CompletedQuests.Contains("q_kill_wolves"));

            Object.DestroyImmediate(wolf1);
        }

        [Test]
        public void Quest_Completes_OnceRequiredKillCountReached_AndGrantsReward()
        {
            questManager.StartQuest("q_kill_wolves");

            var wolf = new GameObject("Wolf");
            var wolfStats = wolf.AddComponent<CharacterStats>();
            wolfStats.characterId = "wolf";
            wolfStats.isPlayer = false;

            Core.GameEvents.RaiseCharacterDied(wolfStats);
            Core.GameEvents.RaiseCharacterDied(wolfStats);
            Core.GameEvents.RaiseCharacterDied(wolfStats);

            Assert.IsFalse(questManager.ActiveQuests.ContainsKey("q_kill_wolves"));
            Assert.IsTrue(questManager.CompletedQuests.Contains("q_kill_wolves"));
            Assert.AreEqual(50, leveling.TotalExperience);

            Object.DestroyImmediate(wolf);
        }

        [Test]
        public void StartQuest_AlreadyActive_CannotBeStartedAgain()
        {
            questManager.StartQuest("q_kill_wolves");
            bool secondStart = questManager.StartQuest("q_kill_wolves");
            Assert.IsFalse(secondStart);
        }

        [Test]
        public void StoryFlag_OnlyRaisesEventOnce_ForSameFlag()
        {
            int fireCount = 0;
            System.Action<string> handler = _ => fireCount++;
            Core.GameEvents.OnStoryFlagSet += handler;

            questManager.SetStoryFlag("met_the_rival");
            questManager.SetStoryFlag("met_the_rival");

            Core.GameEvents.OnStoryFlagSet -= handler;
            Assert.AreEqual(1, fireCount);
        }
    }
}
