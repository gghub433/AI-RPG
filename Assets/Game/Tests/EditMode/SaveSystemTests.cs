using System.IO;
using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.SaveSystem;
using RebirthEclipse.Character;

namespace RebirthEclipse.Tests
{
    /// <summary>
    /// Exercises SaveManager against the real filesystem under Application.persistentDataPath,
    /// using a dedicated slot index so these tests never collide with a real player's save.
    /// </summary>
    public class SaveSystemTests
    {
        private const int TestSlot = 97; // far outside the 0..4 range UI exposes to players

        [TearDown]
        public void TearDown()
        {
            SaveManager.DeleteSlot(TestSlot);
        }

        private static SaveData BuildSampleSave()
        {
            var data = new SaveData
            {
                saveSlotName = "test",
                level = 7,
                totalExperience = 1234,
                baseStats = new StatBlock { strength = 15, agility = 12, vitality = 20, intellect = 8, spirit = 5, manaControl = 4, curseResistance = 3, luck = 6 },
                currentHealth = 88f,
                currentMana = 40f,
                currentStamina = 60f
            };
            data.storyFlags.Add("arc1_betrayal_witnessed");
            data.completedQuestIds.Add("q_prologue_01");
            data.npcAffinity["companion_sera"] = 42;
            data.abyssResistances["Physical"] = 12.5f;
            return data;
        }

        [Test]
        public void SaveThenLoad_RoundTrips_AllFields()
        {
            var original = BuildSampleSave();
            SaveManager.Save(TestSlot, original);

            var loaded = SaveManager.Load(TestSlot);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(original.level, loaded.level);
            Assert.AreEqual(original.totalExperience, loaded.totalExperience);
            Assert.AreEqual(original.baseStats.strength, loaded.baseStats.strength);
            Assert.AreEqual(original.currentHealth, loaded.currentHealth);
            Assert.Contains("arc1_betrayal_witnessed", loaded.storyFlags);
            Assert.Contains("q_prologue_01", loaded.completedQuestIds);
            Assert.AreEqual(42, loaded.npcAffinity["companion_sera"]);
        }

        [Test]
        public void Load_NonExistentSlot_ReturnsNull()
        {
            SaveManager.DeleteSlot(TestSlot + 1);
            var loaded = SaveManager.Load(TestSlot + 1);
            Assert.IsNull(loaded);
        }

        [Test]
        public void Load_CorruptedPrimaryFile_FallsBackToBackup()
        {
            var original = BuildSampleSave();
            SaveManager.Save(TestSlot, original);          // writes slot + rotates (no backup yet, first write)
            SaveManager.Save(TestSlot, original);          // second write creates a valid backup of the first

            string primaryPath = Path.Combine(Application.persistentDataPath, $"save_{TestSlot}.json");
            File.WriteAllText(primaryPath, "{ this is not valid json ][");

            var loaded = SaveManager.Load(TestSlot);

            Assert.IsNotNull(loaded, "A corrupted primary save must fall back to the backup file rather than failing outright.");
            Assert.AreEqual(original.level, loaded.level);
        }

        [Test]
        public void Save_Overwrite_PreservesNewestDataAfterReload()
        {
            var first = BuildSampleSave();
            SaveManager.Save(TestSlot, first);

            var second = BuildSampleSave();
            second.level = 20;
            second.totalExperience = 99999;
            SaveManager.Save(TestSlot, second);

            var loaded = SaveManager.Load(TestSlot);
            Assert.AreEqual(20, loaded.level);
            Assert.AreEqual(99999, loaded.totalExperience);
        }
    }
}
