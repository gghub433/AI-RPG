using NUnit.Framework;
using UnityEngine;
using RebirthEclipse.Inventory;

namespace RebirthEclipse.Tests
{
    public class InventorySystemTests
    {
        private GameObject go;
        private InventorySystem inventory;
        private ItemDatabase database;
        private ItemDefinition potion;
        private ItemDefinition sword;

        [SetUp]
        public void SetUp()
        {
            potion = ScriptableObject.CreateInstance<ItemDefinition>();
            potion.itemId = "potion_health_small";
            potion.maxStack = 10;
            potion.category = ItemCategory.Consumable;

            sword = ScriptableObject.CreateInstance<ItemDefinition>();
            sword.itemId = "sword_iron";
            sword.maxStack = 1;
            sword.category = ItemCategory.Weapon;
            sword.slot = EquipmentSlot.MainHand;

            database = ScriptableObject.CreateInstance<ItemDatabase>();
            database.allItems = new[] { potion, sword };

            go = new GameObject("TestInventory");
            inventory = go.AddComponent<InventorySystem>();
            typeof(InventorySystem)
                .GetField("itemDatabase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(inventory, database);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(go);
            Object.DestroyImmediate(potion);
            Object.DestroyImmediate(sword);
            Object.DestroyImmediate(database);
        }

        [Test]
        public void AddStackable_BelowMaxStack_MergesIntoOneStack()
        {
            inventory.AddStackable("potion_health_small", 3);
            inventory.AddStackable("potion_health_small", 4);
            Assert.AreEqual(7, inventory.CountOf("potion_health_small"));
            Assert.AreEqual(1, inventory.Stacks.Count);
        }

        [Test]
        public void AddStackable_AboveMaxStack_CreatesAdditionalStack()
        {
            inventory.AddStackable("potion_health_small", 15); // maxStack = 10
            Assert.AreEqual(15, inventory.CountOf("potion_health_small"));
            Assert.AreEqual(2, inventory.Stacks.Count);
        }

        [Test]
        public void RemoveStackable_MoreThanOwned_FailsAndLeavesInventoryUnchanged()
        {
            inventory.AddStackable("potion_health_small", 2);
            bool removed = inventory.RemoveStackable("potion_health_small", 5);
            Assert.IsFalse(removed);
            Assert.AreEqual(2, inventory.CountOf("potion_health_small"));
        }

        [Test]
        public void AddUniqueInstance_WithSameInstanceIdTwice_IsRejectedSecondTime()
        {
            bool first = inventory.AddUniqueInstance("sword_iron", "instance-001", default);
            bool second = inventory.AddUniqueInstance("sword_iron", "instance-001", default);

            Assert.IsTrue(first);
            Assert.IsFalse(second, "Adding the same unique item instance twice must be rejected — this is the duplication guard.");
            Assert.AreEqual(1, inventory.CountOf("sword_iron"));
        }

        [Test]
        public void AddUniqueInstance_DifferentIds_BothSucceed()
        {
            inventory.AddUniqueInstance("sword_iron", "instance-A", default);
            inventory.AddUniqueInstance("sword_iron", "instance-B", default);
            Assert.AreEqual(2, inventory.CountOf("sword_iron"));
        }

        [Test]
        public void RemoveUniqueInstance_FreesTheInstanceIdForReuse()
        {
            inventory.AddUniqueInstance("sword_iron", "instance-001", default);
            inventory.RemoveUniqueInstance("instance-001");
            bool readdResult = inventory.AddUniqueInstance("sword_iron", "instance-001", default);
            Assert.IsTrue(readdResult);
        }

        [Test]
        public void SaveLoadRoundTrip_PreservesStacksAndInstanceGuard()
        {
            inventory.AddStackable("potion_health_small", 5);
            inventory.AddUniqueInstance("sword_iron", "instance-001", default);

            var exported = inventory.ExportForSave();

            var go2 = new GameObject("ReloadedInventory");
            var reloaded = go2.AddComponent<InventorySystem>();
            typeof(InventorySystem)
                .GetField("itemDatabase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(reloaded, database);
            reloaded.LoadFromSave(exported);

            Assert.AreEqual(5, reloaded.CountOf("potion_health_small"));
            Assert.AreEqual(1, reloaded.CountOf("sword_iron"));

            // Duplication guard must survive the round trip too.
            bool duplicateAfterLoad = reloaded.AddUniqueInstance("sword_iron", "instance-001", default);
            Assert.IsFalse(duplicateAfterLoad);

            Object.DestroyImmediate(go2);
        }
    }
}
